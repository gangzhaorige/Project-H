package com.zzhgl.app.core;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Map;

import org.mindrot.jbcrypt.BCrypt;

import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.ConfFileParser;
import com.zzhgl.app.utility.Log;

public class DatabaseManager {

    private static DatabaseManager instance;
    private Connection connection;

    private DatabaseManager() {
        try {
            // Read configuration from conf/db.conf
            ConfFileParser confFileParser = new ConfFileParser("conf/db.conf");
            Map<String, String> dbConf = confFileParser.parse();

            String dbUrl = dbConf.get("DBURL");
            String dbName = dbConf.get("DBName");
            String dbUser = dbConf.get("DBUsername");
            String dbPass = dbConf.get("DBPassword");

            // Allow override via environment variables (useful for Docker)
            if (System.getenv("DB_HOST") != null) {
                dbUrl = System.getenv("DB_HOST");
            }

            // Construct the JDBC URL for PostgreSQL
            // Assuming default port 5432 if not specified in dbUrl
            String jdbcUrl = "jdbc:postgresql://" + dbUrl + ":5432/" + dbName;

            // Load the PostgreSQL JDBC driver
            Class.forName("org.postgresql.Driver");
            
            // Connect to the database
            connection = DriverManager.getConnection(jdbcUrl, dbUser, dbPass);
            Log.println("Connected to PostgreSQL database successfully.");
            
            initializeTables();
        } catch (Exception e) {
            Log.println_e("Database Connection Failed: " + e.getMessage());
        }
    }

    public static DatabaseManager getInstance() {
        if (instance == null) {
            instance = new DatabaseManager();
        }
        return instance;
    }

    private void initializeTables() {
        // PostgreSQL uses SERIAL instead of AUTOINCREMENT
        String createPlayersTable = "CREATE TABLE IF NOT EXISTS players ("
                + "id SERIAL PRIMARY KEY,"
                + "username VARCHAR(255) UNIQUE NOT NULL,"
                + "password VARCHAR(255) NOT NULL"
                + ");";

        try (Statement stmt = connection.createStatement()) {
            stmt.execute(createPlayersTable);
            Log.println("Database tables initialized.");
        } catch (SQLException e) {
            Log.println_e("Failed to initialize tables: " + e.getMessage());
        }
    }

    /**
     * Attempts to log in a user. If the user does not exist, registers them.
     * @return Player object on success, null on invalid password.
     */
    public Player loginOrRegister(String username, String password) {
        String selectSql = "SELECT id, password FROM players WHERE username = ?";
        try (PreparedStatement pstmt = connection.prepareStatement(selectSql)) {
            pstmt.setString(1, username);
            ResultSet rs = pstmt.executeQuery();

            if (rs.next()) {
                // User exists, verify password using BCrypt
                String dbPasswordHash = rs.getString("password");
                if (BCrypt.checkpw(password, dbPasswordHash)) {
                    int id = rs.getInt("id");
                    return new Player(id, username);
                } else {
                    Log.println_e("Invalid password for user: " + username);
                    return null; // Invalid password
                }
            } else {
                // User doesn't exist, register new user
                return registerPlayer(username, password);
            }
        } catch (SQLException e) {
            Log.println_e("Database error during login: " + e.getMessage());
            return null;
        }
    }

    private Player registerPlayer(String username, String password) {
        String insertSql = "INSERT INTO players(username, password) VALUES(?, ?)";
        try (PreparedStatement pstmt = connection.prepareStatement(insertSql, Statement.RETURN_GENERATED_KEYS)) {
            pstmt.setString(1, username);
            
            // Hash the password before storing it
            String hashedPassword = BCrypt.hashpw(password, BCrypt.gensalt(12));
            pstmt.setString(2, hashedPassword);
            
            pstmt.executeUpdate();

            ResultSet rs = pstmt.getGeneratedKeys();
            if (rs.next()) {
                int id = rs.getInt(1); // Usually id is the first column in the generated keys
                Log.println("Registered new user: " + username + " with ID: " + id);
                return new Player(id, username);
            }
        } catch (SQLException e) {
            Log.println_e("Failed to register player: " + e.getMessage());
        }
        return null;
    }

    public void close() {
        try {
            if (connection != null && !connection.isClosed()) {
                connection.close();
                Log.println("Database connection closed.");
            }
        } catch (SQLException e) {
            Log.println_e("Failed to close database: " + e.getMessage());
        }
    }
}
