package com.zzhgl.app.networking.request.room;

import java.io.IOException;
import java.util.List;

import com.zzhgl.app.core.RoomManager;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.room.ResponseAllRooms;

public class RequestAllRooms extends GameRequest {

    @Override
    public void parse() throws IOException {
    }

    @Override
    public void doBusiness() throws Exception {
        ResponseAllRooms response = new ResponseAllRooms();
        List<Room> rooms = RoomManager.getInstance().getAllRooms();
        response.setRooms(rooms);
        responses.add(response);
    }
}
