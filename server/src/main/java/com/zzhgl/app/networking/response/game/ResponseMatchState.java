package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseMatchState extends GameResponse {
    private int connectedPlayers;
    private int totalPlayers;

    public ResponseMatchState(int connectedPlayers, int totalPlayers) {
        responseCode = Constants.SMSG_MATCH_STATE;
        this.connectedPlayers = connectedPlayers;
        this.totalPlayers = totalPlayers;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString(""); // empty message
        packet.addInt32(connectedPlayers);
        packet.addInt32(totalPlayers);
        return packet.getBytes();
    }
}
