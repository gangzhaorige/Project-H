package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseTurnStart notifies all players about whose turn it is.
 */
public class ResponseTurnStart extends GameResponse {
    private int activePlayerId;

    public ResponseTurnStart(int activePlayerId) {
        this.responseCode = Constants.SMSG_TURN_START;
        this.activePlayerId = activePlayerId;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString("Turn started for player " + activePlayerId);
        packet.addInt32(activePlayerId);
        return packet.getBytes();
    }
}
