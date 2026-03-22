package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseFieldToHand extends GameResponse {
    private int casterId;
    private AbstractCard card;

    public ResponseFieldToHand(int casterId, AbstractCard card) {
        this.responseCode = Constants.SMSG_FIELD_TO_HAND;
        this.casterId = casterId;
        this.card = card;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addInt32(casterId);
        
        packet.addInt32(card.getId());
        packet.addInt32(card.getSuit().ordinal());
        packet.addInt32(card.getValue());
        
        String type = "Standard";
        if (card instanceof AbstractNormalCard normal) {
            type = normal.getNormalType().name();
        } else if (card instanceof AbstractSpecialCard special) {
            type = special.getSpecialType().name();
        }
        packet.addString(type);
        
        return packet.getBytes();
    }
}
