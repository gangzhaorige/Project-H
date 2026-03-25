package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.utility.GamePacket;

public class ResponseSelectCardsFromOpponent extends GameResponse {
    private final int targetPlayerId;
    private final int targetHandSize;
    private final int amount;
    private final int duration;
    private final String message;

    public ResponseSelectCardsFromOpponent(int targetPlayerId, int amount, int duration, String message, int targetHandSize) {
        this.responseCode = Constants.SMSG_SELECT_CARDS_FROM_OPPONENT;
        this.targetPlayerId = targetPlayerId;
        this.amount = amount;
        this.targetHandSize = targetHandSize;
        this.duration = duration;
        this.message = message;
    }

    public int getTargetPlayerId() { return targetPlayerId; }
    public int getAmount() { return amount; }
    public int getDuration() { return duration; }
    public String getMessage() { return message; }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addInt32(targetHandSize);
        packet.addInt32(targetPlayerId);
        packet.addInt32(amount);
        packet.addInt32(duration);
        packet.addString(message);

        return packet.getBytes();
    }
}
