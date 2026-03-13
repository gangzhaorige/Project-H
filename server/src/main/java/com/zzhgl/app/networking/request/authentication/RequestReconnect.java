package com.zzhgl.app.networking.request.authentication;

import java.io.IOException;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.authentication.ResponseReconnect;
import com.zzhgl.app.utility.Log;

public class RequestReconnect extends GameRequest {

    private String username;
    private String token;

    @Override
    public void parse() throws IOException {
        username = com.zzhgl.app.utility.DataReader.readString(dataInput);
        token = com.zzhgl.app.utility.DataReader.readString(dataInput);
    }

    @Override
    public void doBusiness() throws Exception {
        Player player = client.getPlayer();
        
        // If the client doesn't have a player yet, try to find one by token
        if (player == null) {
            for (Player p : com.zzhgl.app.core.GameServer.getInstance().getActivePlayers()) {
                if (p.getUsername().equals(username) && token.equals(p.getSessionToken())) {
                    player = p;
                    // Associate the new socket with this player
                    player.setClient(client); // Marks as connected and flushes pending updates
                    client.setPlayer(player);
                    break;
                }
            }
        }
        
        ResponseReconnect response = new ResponseReconnect();
        
        if (player == null) {
            response.setStatus(Constants.AUTHENTICATION_FAILED);
            responses.add(response);
            return;
        }

        Room room = player.getCurrentRoom();
        if (room != null) {
            response.setStatus(Constants.SUCCESS);
            response.setRoomId(room.getId());
        
            
            Log.printf("Player %s is reconnecting to room: %s", player.getUsername(), room.getName());
        } else {
            response.setStatus(Constants.FAILED); // Not in a room
            Log.printf("Player %s attempted to reconnect but is not in a room.", player.getUsername());
        }
        
        responses.add(response);
    }
}
