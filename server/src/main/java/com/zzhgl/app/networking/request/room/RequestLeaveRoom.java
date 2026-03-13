package com.zzhgl.app.networking.request.room;

import java.io.IOException;

import com.zzhgl.app.core.RoomManager;
import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.room.ResponseLeaveRoom;
import com.zzhgl.app.utility.Log;

public class RequestLeaveRoom extends GameRequest {

    @Override
    public void parse() throws IOException {
        // No payload for leaving current room
    }

    @Override
    public void doBusiness() throws Exception {
        Player player = client.getPlayer();
        ResponseLeaveRoom response = new ResponseLeaveRoom();
        
        if (player == null) {
            response.setStatus(Constants.AUTHENTICATION_FAILED);
            responses.add(response);
            return;
        }

        Room room = player.getCurrentRoom();
        if (room != null) {
            room.removePlayer(player);
            player.setCurrentRoom(null);
            
            if (room.isEmpty()) {
                RoomManager.getInstance().removeRoom(room.getId());
                Log.printf("Room %s is empty and has been removed.", room.getName());
            } else {
                if (room.getHostPlayerId() == player.getID()) {
                    Player newHost = room.getPlayers().get(0);
                    room.setHostPlayerId(newHost.getID());
                    Log.printf("Host left room %s, new host is %s", room.getName(), newHost.getUsername());
                }
            }
            
            response.setStatus(Constants.SUCCESS);
            Log.printf("Player %s left room: %s", player.getUsername(), room.getName());
        } else {
            response.setStatus(Constants.FAILED); // Not in a room
        }
        
        responses.add(response);
    }
}
