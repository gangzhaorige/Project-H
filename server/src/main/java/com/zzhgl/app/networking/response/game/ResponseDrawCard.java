package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

import java.util.List;

/**
 * ResponseDrawCard sends the actual card data to the player who drew them.
 */
public class ResponseDrawCard extends GameResponse {
    private List<AbstractCard> cards;

    public ResponseDrawCard(List<AbstractCard> cards) {
        this.responseCode = Constants.SMSG_CARD_DRAW;
        this.cards = cards;
    }
    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);

        packet.addShort16((short) cards.size());
        for (AbstractCard card : cards) {
            packet.addInt32(card.getId());
            packet.addInt32(card.getSuit().ordinal());
            packet.addInt32(card.getValue());
            
            // Send the Type string for client visualization
            String type = "Standard";
            if (card instanceof AbstractNormalCard normal) {
                type = normal.getNormalType().name();
            } else if (card instanceof AbstractSpecialCard special) {
                type = special.getSpecialType().name();
            }
            packet.addString(type);
        }

        return packet.getBytes();
    }
}
