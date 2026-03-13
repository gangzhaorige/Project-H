package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseNotifyPlayerSelect extends GameResponse {
    private int playerId;
    private int championId;

    public ResponseNotifyPlayerSelect(int playerId, int championId) {
        this.responseCode = Constants.SMSG_NOTIFY_PLAYER_SELECT;
        this.playerId = playerId;
        this.championId = championId;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString("Player is hovering a champion.");
        packet.addInt32(playerId);
        packet.addInt32(championId);
        return packet.getBytes();
    }
}
