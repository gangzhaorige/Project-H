package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseNotifyForChampionPick extends GameResponse {
    private int activePlayerId;
    private int timeout;

    public ResponseNotifyForChampionPick(int activePlayerId, int timeout) {
        responseCode = Constants.SMSG_NOTIFY_FOR_CHAMPION_PICK;
        this.activePlayerId = activePlayerId;
        this.timeout = timeout;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);

        packet.addInt32(activePlayerId);        packet.addInt32(timeout);

        return packet.getBytes();
    }
}
