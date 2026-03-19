package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseGameStart extends GameResponse {
    
    public ResponseGameStart() {
        responseCode = Constants.SMSG_GAME_START;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        return packet.getBytes();
    }
}
