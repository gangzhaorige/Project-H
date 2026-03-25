package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;

public class SkillResponseCommand implements Command {
    private int playerId;
    private boolean accepted;
    private int skillId;

    public SkillResponseCommand(int playerId, boolean accepted, int skillId) {
        this.playerId = playerId;
        this.accepted = accepted;
        this.skillId = skillId;
    }

    public int getPlayerId() { return playerId; }
    public boolean isAccepted() { return accepted; }
    public int getSkillId() { return skillId; }

    @Override
    public void execute(GameManager game) {
        game.handleAction(this);
    }
}
