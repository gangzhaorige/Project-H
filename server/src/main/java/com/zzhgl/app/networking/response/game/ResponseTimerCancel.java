package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseTimerCancel notifies all players that the current turn timer is cancelled.
 */
public class ResponseTimerCancel extends GameResponse {

    public ResponseTimerCancel() {
        this.responseCode = Constants.SMSG_RESPONSE_TIMER_CANCEL;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        return packet.getBytes();
    }
}
