package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

import java.util.Collection;

/**
 * ResponseGameSetup sends the finalized player data and champion stats 
 * to clients so they can reconstruct the game objects.
 */
public class ResponseGameSetup extends GameResponse {
    private Collection<Player> players;

    public ResponseGameSetup(Collection<Player> players) {
        this.responseCode = Constants.SMSG_GAME_SETUP;
        this.players = players;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addString(""); // Add empty message
        
        packet.addShort16((short) players.size());
        System.out.println("---------------- ---------------" + players.size());
        for (Player player : players) {
            packet.addInt32(player.getID());
            packet.addString(player.getUsername());
            packet.addInt32(player.getTeam());
            
            Champion champ = player.getSelectedChampion();
            if (champ != null) {
                packet.addInt32(champ.getId());
                packet.addString(champ.getChampionName());
                packet.addInt32(champ.getMaxHP());
                packet.addInt32(champ.getCurHP());
                packet.addInt32(champ.getPathId());
                packet.addString(champ.getElement());
                packet.addInt32(champ.getAttack());
                packet.addInt32(champ.getAttackRange());
            } else {
                packet.addInt32(-1); // No champion
            }
        }

        return packet.getBytes();
    }
}
