package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

import java.util.List;

/**
 * ResponseMoveCard notifies players about cards moving from one player to another.
 * Can hide card information for observers.
 */
public class ResponseMoveCard extends GameResponse {
    private final List<AbstractCard> cards;
    private final int casterId; // Receiver
    private final int targetId;  // Source/Loser
    private final boolean showDetails;

    public ResponseMoveCard(List<AbstractCard> cards, int casterId, int targetId, boolean showDetails) {
        this.responseCode = Constants.SMSG_MOVE_CARD;
        this.cards = cards;
        this.casterId = casterId;
        this.targetId = targetId;
        this.showDetails = showDetails;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        
        packet.addInt32(casterId);
        packet.addInt32(targetId);
        packet.addBoolean(showDetails);
        
        packet.addShort16((short) cards.size());
        for (AbstractCard card : cards) {
            if (showDetails) {
                packet.addInt32(card.getId());
                packet.addInt32(card.getSuit().ordinal());
                packet.addInt32(card.getValue());
                packet.addInt32(card.getEnumId());
            } else {
                // Observers only see placeholders
                packet.addInt32(-1);
                packet.addInt32(0);
                packet.addInt32(0);
                packet.addInt32(0); // 0 for Hidden/Unknown type
            }
        }

        return packet.getBytes();
    }
}
