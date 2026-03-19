package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

import java.util.Collection;

public class ResponseChampionSelectReady extends GameResponse {
    private Collection<Player> players;
    private Collection<Integer> championPool;

    public ResponseChampionSelectReady() {
        responseCode = Constants.SMSG_CHAMPION_SELECT_READY;
    }

    public void setPlayers(Collection<Player> players) {
        this.players = players;
    }

    public void setChampionPool(Collection<Integer> championPool) {
        this.championPool = championPool;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        
        // Players
        packet.addShort16((short) players.size());
        for (Player player : players) {
            packet.addInt32(player.getID());
            packet.addString(player.getUsername());
            packet.addInt32(player.getTeam());
        }

        // Champion Pool
        if (championPool != null) {
            packet.addShort16((short) championPool.size());
            for (int champId : championPool) {
                packet.addInt32(champId);
            }
        } else {
            packet.addShort16((short) 0);
        }

        return packet.getBytes();
    }
}
