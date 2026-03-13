package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseStartChampionSelection extends GameResponse {
    public ResponseStartChampionSelection() {
        this.responseCode = Constants.SMSG_START_CHAMPION_SELECTION;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString("Champion selection phase started.");
        return packet.getBytes();
    }
}
