package com.zzhgl.app.networking.response.room;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseJoinRoom extends GameResponse {
    private short status;
    private String roomId = "";
    private String roomName = "";

    public ResponseJoinRoom() {
        responseCode = Constants.SMSG_JOIN_ROOM;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(status);
        packet.addString(""); // Empty message
        if (status == Constants.SUCCESS) {
            packet.addString(roomId);
            packet.addString(roomName);
        }
        return packet.getBytes();
    }

    public void setStatus(short status) { this.status = status; }
    public void setRoomId(String roomId) { this.roomId = roomId; }
    public void setRoomName(String roomName) { this.roomName = roomName; }
}
