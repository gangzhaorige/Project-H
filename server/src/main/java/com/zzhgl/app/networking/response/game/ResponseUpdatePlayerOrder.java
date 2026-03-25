package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;
import java.util.List;

public class ResponseUpdatePlayerOrder extends GameResponse {
    private final List<Integer> playerIds;

    public ResponseUpdatePlayerOrder(List<Integer> playerIds) {
        this.responseCode = Constants.SMSG_UPDATE_PLAYER_ORDER;
        this.playerIds = playerIds;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addShort16((short) playerIds.size());
        for (int id : playerIds) {
            packet.addInt32(id);
        }
        return packet.getBytes();
    }
}
