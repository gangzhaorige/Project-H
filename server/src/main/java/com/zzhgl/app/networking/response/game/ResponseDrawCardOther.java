package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseDrawCardOther notifies other players how many cards a player drew.
 */
public class ResponseDrawCardOther extends GameResponse {
    private int playerId;
    private int cardCount;

    public ResponseDrawCardOther(int playerId, int cardCount) {
        this.responseCode = Constants.SMSG_CARD_DRAW_OTHER;
        this.playerId = playerId;
        this.cardCount = cardCount;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);

        packet.addInt32(playerId);        packet.addInt32(cardCount);

        return packet.getBytes();
    }
}
