package com.zzhgl.app.networking.response.system;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseHeartbeat extends GameResponse {

    public ResponseHeartbeat() {
        responseCode = Constants.SMSG_HEARTBEAT;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16((short) 0); // Status 0 (SUCCESS)
        packet.addString(""); // Empty message
        return packet.getBytes();
    }
}
