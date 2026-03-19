package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponsePassPriority notifies a player that their pass was successful.
 */
public class ResponsePassPriority extends GameResponse {

    public ResponsePassPriority() {
        this.responseCode = Constants.SMSG_PASS_PRIORITY;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        return packet.getBytes();
    }
}
