package com.zzhgl.app.networking.request.room;

import java.io.IOException;

import com.zzhgl.app.core.RoomManager;
import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.room.ResponseCreateRoom;
import com.zzhgl.app.utility.DataReader;
import com.zzhgl.app.utility.Log;

public class RequestCreateRoom extends GameRequest {
    private String roomName;

    @Override
    public void parse() throws IOException {
        roomName = DataReader.readString(dataInput);
    }

    @Override
    public void doBusiness() throws Exception {
        Player player = client.getPlayer();
        ResponseCreateRoom response = new ResponseCreateRoom();
        
        // Trim to prevent whitespace issues
        String cleanName = roomName != null ? roomName.trim() : "";
        Log.printf("Create Room Request: player '%s' wants to create room '%s'", 
            player != null ? player.getUsername() : "unknown", cleanName);
        
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

        if (cleanName.isEmpty()) {
            response.setStatus(Constants.INVALID_ROOM_NAME);
            responses.add(response);
            return;
        }

        Room room = RoomManager.getInstance().createRoom(cleanName, player);
        if (room != null) {
            player.setCurrentRoom(room);
            response.setStatus(Constants.SUCCESS);
            response.setRoomId(room.getId());
            response.setRoomName(room.getName());
            Log.printf("Player %s successfully created room: %s", player.getUsername(), cleanName);
        } else {
            Log.printf("Create Room Failed: Room '%s' already exists.", cleanName);
            response.setStatus(Constants.DUPLICATE_ROOM_NAME);
        }
        
        responses.add(response);
    }
}
