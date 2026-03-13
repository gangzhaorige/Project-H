package com.zzhgl.app.core;


// Java Imports
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.lang.reflect.Field;
import java.net.Socket;
import java.net.SocketTimeoutException;
import java.util.LinkedList;
import java.util.Queue;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.metadata.GameRequestTable;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.DataReader;
import com.zzhgl.app.utility.Log;

// Other Imports


/**
 * The GameClient class is an extension of the Thread class that represents an
 * individual client. Not only does this class holds the connection between the
 * client and server, it is also in charge of managing the connection to
 * actively receive incoming requests and send outgoing responses. This thread
 * lasts as long as the connection is alive.
 */
public class GameClient implements Runnable {

    // Client Variables
    private String session_id;
    private Socket clientSocket;
    private InputStream inputStream; // For use with incoming requests
    private OutputStream outputStream; // For use with outgoing responses
    private DataInputStream dataInputStream; // Stores incoming requests for use
    private boolean isDone;
    // Other Variables
    private Player player;

    /**
     * Initialize the GameClient using the client socket and creating both input
     * and output streams.
     * 
     * @param session_id holds the unique identifier of this session
     * @param clientSocket holds reference of the socket being used
     * @throws IOException 
     */
    public GameClient(String session_id, Socket clientSocket) throws IOException {
        this.session_id = session_id;
        this.clientSocket = clientSocket;
        inputStream = clientSocket.getInputStream();
        outputStream = clientSocket.getOutputStream();
        dataInputStream = new DataInputStream(inputStream);
    }

    /**
     * Holds the main loop that processes incoming requests by first identifying
     * its type, then interpret the following data in each determined request
     * class. Queued up responses created from each request class will be sent
     * after the request is finished processing.
     * 
     * The loop exits whenever the isPlaying flag is set to false. One of these
     * occurrences is triggered by a timeout. A timeout occurs whenever no
     * activity is picked up from the client such as being disconnected.
     */
    @Override
    public void run() {
        long lastActivity = System.currentTimeMillis();
        short requestCode = -1;

        try {
            while (!isDone) {
                try {
                    // Check for incoming data
                    short requestLength = DataReader.readShort(dataInputStream);

                    if (requestLength > 0) {
                        lastActivity = System.currentTimeMillis();

                        // Process request (existing logic)
                        byte[] buffer = new byte[requestLength];
                        inputStream.read(buffer, 0, requestLength);
                        DataInputStream dataInput = new DataInputStream(new ByteArrayInputStream(buffer));
                        requestCode = DataReader.readShort(dataInput);
                        GameRequest request = GameRequestTable.get(requestCode);

                        if (request != null) {
                            request.setGameClient(this);
                            request.setDataInputStream(dataInput);
                            request.parse();
                            request.doBusiness();

                            try {
                                for (GameResponse response : request.getResponses()) {
                                    send(response);
                                }
                            } catch (IOException ex) {
                                Log.printf_e("Client %s connection lost during send", session_id);
                                isDone = true;
                            }
                        }
                    } else {
                        long currentInactive = (System.currentTimeMillis() - lastActivity) / 1000;
                        
                        if (currentInactive >= Constants.TIMEOUT_SECONDS) {
                            Log.printf("Client %s timed out due to inactivity", session_id);
                            if (player != null) {
                                player.setConnected(false);
                            }
                            isDone = true;
                        }
                    }
                } catch (SocketTimeoutException ste) {
                    // Intentional empty catch for non-blocking read
                } catch (IOException ioe) {
                    // Log the error but check if we should really exit
                    Log.printf_e("Client %s I/O Error: %s", session_id, ioe.getMessage());
                    isDone = true;
                } catch (Exception ex) {
                    Log.printf_e("Request Error: %s", ex.getMessage());
                    isDone = true;
                }
            }
        } finally {
            if (player != null) {
                player.setConnected(false);
                player.setDisconnectedTime(System.currentTimeMillis());
            }
            
            try {
                if (!clientSocket.isClosed()) {
                    clientSocket.close();
                }
            } catch (IOException e) {
                Log.println_e("Error closing socket: " + e.getMessage());
            }

            GameServer.getInstance().deletePlayerThreadOutOfActiveThreads(session_id);
            Log.printf("Client %s has ended", session_id);
        }
    }

    public String getID() {
        return session_id;
    }

    public void end() {
        isDone = true;
    }

    public int getUserID() {
        return player != null ? player.getID() : -1;
    }

    public Player getPlayer() {
        return player;
    }

    public Player setPlayer(Player player) {
        return this.player = player;
    }

    public boolean addResponseForUpdate(GameResponse response) {
        if (player != null) {
            return player.addResponseForUpdate(response);
        }
        return false;
    }

    public synchronized void send(GameResponse response) throws IOException {
        outputStream.write(response.constructResponseInBytes());
    }

    /**
     * Get all pending responses for this client.
     * 
     * @return all pending responses
     */
    public Queue<GameResponse> getUpdates() {
        if (player != null) {
            return player.getUpdates();
        }
        return new LinkedList<GameResponse>();
    }

    public OutputStream getOutputStream() {
        return outputStream;
    }

    /**
     * Remove all responses for this client.
     */
    public void clearUpdateBuffer() {
        if (player != null) {
            player.getUpdates(); // Clearing by retrieving and discarding
        }
    }

    public String getIP() {
        return clientSocket.getInetAddress().getHostAddress();
    }
    
    public void newSession() {
        session_id = GameServer.createUniqueID();
        player = null;
    }

    @Override
    public String toString() {
        String str = "";

        str += "-----" + "\n";
        str += getClass().getName() + "\n";
        str += "\n";

        for (Field field : getClass().getDeclaredFields()) {
            try {
                str += field.getName() + " - " + field.get(this) + "\n";
            } catch (Exception ex) { 
                System.out.println(ex.getMessage());
            }
        }

        str += "-----";

        return str;
    }
}
