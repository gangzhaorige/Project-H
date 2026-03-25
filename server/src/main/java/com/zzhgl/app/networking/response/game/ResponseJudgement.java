package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseJudgement extends GameResponse {
    private AbstractCard card;
    private boolean triggered;

    public ResponseJudgement(AbstractCard card, boolean triggered) {
        this.responseCode = Constants.SMSG_JUDGE;
        this.card = card;
        this.triggered = triggered;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        
        packet.addInt32(card.getId());
        packet.addInt32(card.getSuit().ordinal());
        packet.addInt32(card.getValue());
        
        packet.addInt32(card.getEnumId());
        
        packet.addBoolean(triggered);
        
        return packet.getBytes();
    }
}
