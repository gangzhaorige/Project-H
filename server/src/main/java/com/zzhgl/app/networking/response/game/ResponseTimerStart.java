package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseTimerStart notifies all players that a turn timer has started for a specific player.
 */
public class ResponseTimerStart extends GameResponse {
    private int playerId;
    private int seconds;
    private String message;
    private int requiredCardType;

    public ResponseTimerStart(int playerId, int seconds, String message, Object cardType) {
        this.responseCode = Constants.SMSG_RESPONSE_TIMER_START;
        this.playerId = playerId;
        this.seconds = seconds;
        this.message = message;
        
        if (cardType instanceof AbstractNormalCard.NormalType normal) {
            this.requiredCardType = normal.getId();
        } else if (cardType instanceof AbstractSpecialCard.SpecialType special) {
            this.requiredCardType = special.getId();
        } else if ("ANY".equals(cardType)) {
            this.requiredCardType = -1;
        } else {
            this.requiredCardType = 0;
        }
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addInt32(playerId);
        packet.addInt32(seconds);
        packet.addString(message);
        packet.addInt32(requiredCardType);
        return packet.getBytes();
    }
}
