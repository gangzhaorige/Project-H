package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseSkillQuery asks a player if they want to activate an optional skill
 * or needs them to provide input for a skill.
 */
public class ResponseSkillQuery extends GameResponse {
    private int playerId;
    private int skillId;
    private String skillName;

    public ResponseSkillQuery(int playerId, int skillId, String skillName) {
        this.responseCode = Constants.SMSG_SKILL_QUERY;
        this.playerId = playerId;
        this.skillId = skillId;
        this.skillName = skillName;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addInt32(playerId);
        packet.addInt32(skillId);
        packet.addString(skillName);
        return packet.getBytes();
    }
}
