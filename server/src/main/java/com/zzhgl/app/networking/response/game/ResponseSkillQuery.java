package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

/**
 * ResponseSkillQuery asks a player if they want to activate an optional skill
 * or needs them to provide input for a skill.
 */
public class ResponseSkillQuery extends GameResponse {
    private int skillId;
    private String skillName;

    public ResponseSkillQuery(int skillId, String skillName) {
        this.responseCode = Constants.SMSG_SKILL_QUERY;
        this.skillId = skillId;
        this.skillName = skillName;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addInt32(skillId);
        packet.addString(skillName);
        // packet.addString(sourceName);
        return packet.getBytes();
    }
}
