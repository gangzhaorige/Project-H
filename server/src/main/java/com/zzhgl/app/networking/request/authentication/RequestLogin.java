package com.zzhgl.app.networking.request.authentication;

import java.io.IOException;

import com.zzhgl.app.core.DatabaseManager;
import com.zzhgl.app.core.GameClient;
import com.zzhgl.app.core.GameServer;
import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.authentication.ResponseLogin;
import com.zzhgl.app.utility.DataReader;
import com.zzhgl.app.utility.Log;

public class RequestLogin extends GameRequest {

    private String username;
    private String password;

    @Override
    public void parse() throws IOException {
        username = DataReader.readString(dataInput);
        password = DataReader.readString(dataInput);
    }

    @Override
    public void doBusiness() throws Exception {
        Log.printf("User login attempt: %s", username);
        Player dbPlayer = DatabaseManager.getInstance().loginOrRegister(username, password);
        
        ResponseLogin response = new ResponseLogin();
        
        if (dbPlayer != null) {
            // 2. Check if the player is already active in the server
            Player activePlayer = GameServer.getInstance().getActivePlayer(dbPlayer.getID());
            
            if (activePlayer != null) {
                // Handle reconnection: Kick previous session if it exists and is different
                GameClient oldClient = activePlayer.getClient();
                if (oldClient != null && oldClient != client) {
                    Log.printf("Player %s logged in from new session. Closing old session %s", 
                            username, oldClient.getID());
                    oldClient.end(); // Signal old client to stop
                }
                // Use the existing active player object to preserve state
                activePlayer.setClient(client); // This now marks as connected and flushes
                client.setPlayer(activePlayer);
            } else {
                // New login: Initialize the player object
                dbPlayer.setClient(client);
                dbPlayer.setConnected(true);
                client.setPlayer(dbPlayer);
                GameServer.getInstance().setActivePlayer(dbPlayer);
                activePlayer = dbPlayer;
            }

            // Generate and set session token for this login session
            String token = java.util.UUID.randomUUID().toString();
            activePlayer.setSessionToken(token);
            
            response.setStatus(Constants.SUCCESS);
            response.setPlayerId(activePlayer.getID());
            response.setUsername(username);
            response.setSessionToken(token);
            
            com.zzhgl.app.model.core.Room room = activePlayer.getCurrentRoom();
            if (room != null) {
                response.setRoomId(room.getId());
                response.setInGame(room.isInGame());
            } else {
                response.setRoomId("");
                response.setInGame(false);
            }
        } else {
            response.setStatus(Constants.AUTHENTICATION_FAILED);
        }
        
        responses.add(response);
    }
}
