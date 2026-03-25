package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;
import java.util.List;

public class ActivateSkillCommand implements Command {
    private final int playerId;
    private final int skillId;
    private final List<Integer> discardCardIds;
    private final List<Integer> targetIds;

    public ActivateSkillCommand(int playerId, int skillId, List<Integer> discardCardIds, List<Integer> targetIds) {
        this.playerId = playerId;
        this.skillId = skillId;
        this.discardCardIds = discardCardIds;
        this.targetIds = targetIds;
    }

    public int getPlayerId() { return playerId; }
    public int getSkillId() { return skillId; }
    public List<Integer> getDiscardCardIds() { return discardCardIds; }
    public List<Integer> getTargetIds() { return targetIds; }

    @Override
    public void execute(GameManager game) {
        // Handled in state handleAction
    }
}
