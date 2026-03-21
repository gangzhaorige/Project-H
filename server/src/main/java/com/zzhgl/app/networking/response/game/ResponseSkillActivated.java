package com.zzhgl.app.networking.response.game;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseSkillActivated extends GameResponse {
    private int playerId;
    private int skillIndex;

    public ResponseSkillActivated(int playerId, int skillIndex) {
        this.responseCode = Constants.SMSG_SKILL_ACTIVATION;
        this.playerId = playerId;
        this.skillIndex = skillIndex;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(Constants.SUCCESS);
        packet.addInt32(playerId);
        packet.addInt32(skillIndex);
        return packet.getBytes();
    }
}
