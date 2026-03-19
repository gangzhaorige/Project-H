package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

import java.util.List;

/**
 * ResponsePlayCard notifies all players about a card being played.
 */
public class ResponsePlayCard extends GameResponse {
    private int playerId;
    private int cardId;
    private int suit;
    private int value;
    private String cardType;
    private List<Integer> targetIds;

    public ResponsePlayCard(int playerId, AbstractCard card, List<Integer> targetIds) {
        this.responseCode = Constants.SMSG_PLAY_CARD;
        this.playerId = playerId;
        this.cardId = card.getId();
        this.suit = card.getSuit().ordinal();
        this.value = card.getValue();
        this.targetIds = targetIds;
        
        this.cardType = "Standard";
        if (card instanceof AbstractNormalCard normal) {
            this.cardType = normal.getNormalType().name();
        } else if (card instanceof AbstractSpecialCard special) {
            this.cardType = special.getSpecialType().name();
        }
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString("Player " + playerId + " played card " + cardId);
        
        packet.addInt32(playerId);
        packet.addInt32(cardId);
        packet.addInt32(suit);
        packet.addInt32(value);
        packet.addString(cardType);
        
        packet.addShort16((short) targetIds.size());
        for (int id : targetIds) {
            packet.addInt32(id);
        }

        return packet.getBytes();
    }
}
