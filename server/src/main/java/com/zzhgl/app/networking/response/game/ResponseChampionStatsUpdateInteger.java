package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseChampionStatsUpdateInteger notifies clients about a champion stat change.
 */
public class ResponseChampionStatsUpdateInteger extends GameResponse {
    private int championId;
    private int statId;
    private int value;

    public ResponseChampionStatsUpdateInteger(int championId, int statId, int value) {
        this.responseCode = Constants.SMSG_CHAMPION_STATS_UPDATE_INTEGER;
        this.championId = championId;
        this.statId = statId;
        this.value = value;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addInt32(championId);
        packet.addInt32(statId);
        packet.addInt32(value);
        return packet.getBytes();
    }
}
