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
    private int cardType;
    private List<Integer> targetIds;
    private boolean showJudge;
    private boolean judgeResult;

    public ResponsePlayCard(int playerId, AbstractCard card, List<Integer> targetIds) {
        this(playerId, card, targetIds, false, false);
    }

    public ResponsePlayCard(int playerId, AbstractCard card, List<Integer> targetIds, boolean showJudge, boolean judgeResult) {
        this.responseCode = Constants.SMSG_PLAY_CARD;
        this.playerId = playerId;
        this.cardId = card.getId();
        this.suit = card.getSuit().ordinal();
        this.value = card.getValue();
        this.targetIds = targetIds;
        this.showJudge = showJudge;
        this.judgeResult = judgeResult;
        
        this.cardType = card.getEnumId();
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        
        packet.addInt32(playerId);
        packet.addInt32(cardId);
        packet.addInt32(suit);
        packet.addInt32(value);
        packet.addInt32(cardType);
        
        packet.addShort16((short) targetIds.size());
        for (int id : targetIds) {
            packet.addInt32(id);
        }

        packet.addBoolean(showJudge);
        packet.addBoolean(judgeResult);

        return packet.getBytes();
    }
}
