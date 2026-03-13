package com.zzhgl.app.networking.request.room;

import java.io.IOException;

import com.zzhgl.app.core.RoomManager;
import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.room.ResponseJoinRoom;
import com.zzhgl.app.networking.response.room.ResponseJoinRoomExisting;
import com.zzhgl.app.utility.DataReader;
import com.zzhgl.app.utility.Log;

public class RequestJoinRoom extends GameRequest {
    private String roomId;

    @Override
    public void parse() throws IOException {
        roomId = DataReader.readString(dataInput);
    }

    @Override
    public void doBusiness() throws Exception {
        Player player = client.getPlayer();
        ResponseJoinRoom response = new ResponseJoinRoom();
        
        // Trim to prevent whitespace issues
        String searchKey = roomId != null ? roomId.trim() : "";
        Log.printf("Join Room Request: player '%s' searching for room '%s'", 
            player != null ? player.getUsername() : "unknown", searchKey);
        
        if (player == null) {
            response.setStatus(Constants.AUTHENTICATION_FAILED);
            responses.add(response);
            return;
        }

        if (player.getCurrentRoom() != null) {
            response.setStatus(Constants.ALREADY_IN_ROOM);
            responses.add(response);
            return;
        }

        // Try searching by Name first, then fall back to ID
        Room room = RoomManager.getInstance().getRoomByName(searchKey);
        if (room == null) {
            room = RoomManager.getInstance().getRoomById(searchKey);
        }

        if (room != null) {
            room.addPlayer(player);
            player.setCurrentRoom(room);
            
            response.setStatus(Constants.SUCCESS);
            response.setRoomId(room.getId());
            response.setRoomName(room.getName());
            
            ResponseJoinRoomExisting notifyOthers = new ResponseJoinRoomExisting(player.getID(), player.getUsername());
            room.broadcast(notifyOthers, player.getID());
            
            Log.printf("Player %s successfully joined room: %s", player.getUsername(), room.getName());
        } else {
            Log.printf("Join Room Failed: No room found for key '%s'", searchKey);
            response.setStatus(Constants.FAILED);
        }
        
        responses.add(response);
    }
}
