package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseDiscardCard notifies all players about a card being discarded (e.g. Judgement Override).
 */
public class ResponseDiscardCard extends GameResponse {
    private int playerId;
    private int cardId;
    private int suit;
    private int value;
    private String cardType;
    private boolean showJudge;
    private boolean judgeResult;

    public ResponseDiscardCard(int playerId, AbstractCard card, boolean showJudge, boolean judgeResult) {
        this.responseCode = Constants.SMSG_DISCARD_CARDS;
        this.playerId = playerId;
        this.cardId = card.getId();
        this.suit = card.getSuit().ordinal();
        this.value = card.getValue();
        this.showJudge = showJudge;
        this.judgeResult = judgeResult;
        
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
        
        packet.addInt32(playerId);
        packet.addInt32(cardId);
        packet.addInt32(suit);
        packet.addInt32(value);
        packet.addString(cardType);
        packet.addBoolean(showJudge);
        packet.addBoolean(judgeResult);

        return packet.getBytes();
    }
}
