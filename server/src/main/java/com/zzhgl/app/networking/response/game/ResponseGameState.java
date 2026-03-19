package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseGameState extends GameResponse {
    private String stateName;

    public ResponseGameState(String stateName) {
        this.responseCode = Constants.SMSG_STATE_CHANGE; // Ensure this is defined in Constants
        this.stateName = stateName;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString("");
        packet.addString(stateName); // The core data being broadcast
        return packet.getBytes();
    }
}