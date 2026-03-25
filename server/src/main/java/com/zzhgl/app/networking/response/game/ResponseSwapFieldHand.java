package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseSwapFieldHand extends GameResponse {
    private int casterId;
    private AbstractCard swappedCard;
    private AbstractCard playedCard;
    private boolean judgeResult;

    public ResponseSwapFieldHand(int casterId, AbstractCard swappedCard, AbstractCard playedCard, boolean judgeResult) {
        this.responseCode = Constants.SMSG_SWAP_FIELD_HAND;
        this.casterId = casterId;
        this.swappedCard = swappedCard;
        this.playedCard = playedCard;
        this.judgeResult = judgeResult;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addInt32(casterId);
        
        // Swapped Card (Field -> Hand)
        addCardToPacket(packet, swappedCard);
        
        // Played Card (Hand -> Field)
        addCardToPacket(packet, playedCard);
        
        packet.addBoolean(judgeResult);
        
        return packet.getBytes();
    }

    private void addCardToPacket(GamePacket packet, AbstractCard card) {
        packet.addInt32(card.getId());
        packet.addInt32(card.getSuit().ordinal());
        packet.addInt32(card.getValue());
        packet.addInt32(card.getEnumId());
    }
}
