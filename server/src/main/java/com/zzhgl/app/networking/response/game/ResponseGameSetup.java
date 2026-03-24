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

        packet.addShort16((short) players.size());        int index = 0;
        for (Player player : players) {
            packet.addInt32(player.getID());
            packet.addString(player.getUsername());
            packet.addInt32(player.getTeam());
            packet.addInt32(index++);
            
            Champion champ = player.getSelectedChampion();
            if (champ != null) {
                packet.addInt32(champ.getId());
                packet.addString(champ.getChampionName());
                packet.addInt32(champ.getMaxHP());
                packet.addInt32(champ.getCurHP());
                packet.addInt32(champ.getPath().getId());
                packet.addInt32(champ.getElement().getId());
                packet.addInt32(champ.getAttack());
                packet.addInt32(champ.getAttackRange());
                packet.addInt32(champ.getSpecialDefenseRange());
                packet.addInt32(champ.getAdditionalTargetForAttack());
                packet.addInt32(champ.getMaxNumOfAttack());
                packet.addInt32(champ.getCurNumOfAttack());

                // Add skills
                java.util.List<com.zzhgl.app.model.skills.AbstractSkill> skills = champ.getSkills();
                packet.addInt32(skills != null ? skills.size() : 0);
                if (skills != null) {
                    for (com.zzhgl.app.model.skills.AbstractSkill skill : skills) {
                        packet.addInt32(skill.getId());
                    }
                }
            } else {
                packet.addInt32(-1); // No champion
            }
        }

        return packet.getBytes();
    }
}
