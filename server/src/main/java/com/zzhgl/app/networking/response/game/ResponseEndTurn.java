package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseEndTurn notifies all players that a turn has ended.
 */
public class ResponseEndTurn extends GameResponse {
    private int endedPlayerId;

    public ResponseEndTurn(int endedPlayerId) {
        this.responseCode = Constants.SMSG_END_TURN;
        this.endedPlayerId = endedPlayerId;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString("Turn ended for player " + endedPlayerId);
        packet.addInt32(endedPlayerId);
        return packet.getBytes();
    }
}
