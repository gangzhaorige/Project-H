package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseChampionSelectCompleted extends GameResponse {
    public ResponseChampionSelectCompleted() {
        this.responseCode = Constants.SMSG_CHAMPION_SELECT_COMPLETED;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString("Champion selection completed.");
        return packet.getBytes();
    }
}
