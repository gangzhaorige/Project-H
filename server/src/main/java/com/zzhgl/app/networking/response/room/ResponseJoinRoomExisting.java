package com.zzhgl.app.networking.response.room;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseJoinRoomExisting extends GameResponse {
    private int playerId;
    private String username;

    public ResponseJoinRoomExisting(int playerId, String username) {
        responseCode = Constants.SMSG_JOIN_ROOM_EXISTING;
        this.playerId = playerId;
        this.username = username;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16((short)0); // Success status
        packet.addString(""); // Empty message
        packet.addInt32(playerId);
        packet.addString(username);
        return packet.getBytes();
    }
}
