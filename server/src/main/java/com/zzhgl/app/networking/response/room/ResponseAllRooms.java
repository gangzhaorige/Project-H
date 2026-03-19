package com.zzhgl.app.networking.response.room;

import java.util.List;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseAllRooms extends GameResponse {
    private List<Room> rooms;

    public ResponseAllRooms() {
        responseCode = Constants.SMSG_ALL_ROOMS;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16((short)0); // Status
        
        if (rooms != null) {
            packet.addShort16((short) rooms.size());
            for (Room room : rooms) {
                packet.addString(room.getId());
                packet.addString(room.getName());
                packet.addInt32(room.getPlayers().size());
            }
        } else {
            packet.addShort16((short) 0);
        }
        return packet.getBytes();
    }

    public void setRooms(List<Room> rooms) {
        this.rooms = rooms;
    }
}
