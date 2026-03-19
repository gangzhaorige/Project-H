package com.zzhgl.app.networking.response.authentication;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseReconnect extends GameResponse {
    private short status;
    private String roomId = "";
    // private byte[] serializedGameState; // Placeholder for future full state sync

    public ResponseReconnect() {
        responseCode = Constants.SMSG_RECONNECT;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(status);

        if (status == Constants.SUCCESS) {            
            packet.addString(roomId);
        }
        return packet.getBytes();
    }

    public void setStatus(short status) { this.status = status; }
    public void setRoomId(String roomId) { this.roomId = roomId; }
}
