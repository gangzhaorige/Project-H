package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseTimerStart notifies all players that a turn timer has started for a specific player.
 */
public class ResponseTimerStart extends GameResponse {
    private int playerId;
    private int seconds;
    private String message;

    public ResponseTimerStart(int playerId, int seconds, String message) {
        this.responseCode = Constants.SMSG_RESPONSE_TIMER_START;
        this.playerId = playerId;
        this.seconds = seconds;
        this.message = message;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString("message");
        packet.addInt32(playerId);
        packet.addInt32(seconds);
        packet.addString(message); // Instructional message for client
        return packet.getBytes();
    }
}
