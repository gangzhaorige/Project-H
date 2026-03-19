package com.zzhgl.app.networking.response.room;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseLeaveRoom extends GameResponse {
    private short status;

    public ResponseLeaveRoom() {
        responseCode = Constants.SMSG_LEAVE_ROOM;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(status);
        return packet.getBytes();
    }

    public void setStatus(short status) { this.status = status; }
}
