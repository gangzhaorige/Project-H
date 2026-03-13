package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseGameEnd extends GameResponse {
    private short status = Constants.SUCCESS;
    private String message = "The game has ended.";

    public ResponseGameEnd() {
        responseCode = Constants.SMSG_GAME_END;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(status);
        packet.addString(message);
        // Note: You can append winner ID, scores, or rewards here in the future
        return packet.getBytes();
    }
}
