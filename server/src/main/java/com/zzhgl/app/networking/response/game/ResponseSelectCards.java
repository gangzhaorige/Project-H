package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseSelectCards confirms that the card selection was received.
 */
public class ResponseSelectCards extends GameResponse {

    public ResponseSelectCards() {
        this.responseCode = Constants.SMSG_SELECT_CARDS;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        return packet.getBytes();
    }
}
