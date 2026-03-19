package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;

public class PassPriorityCommand implements Command {
    private int playerId;

    public PassPriorityCommand(int playerId) {
        this.playerId = playerId;
    }

    public int getPlayerId() {
        return playerId;
    }

    @Override
    public void execute(GameManager game) {
        // Handled by the state
    }
}
