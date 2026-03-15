package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;

public class EndTurnCommand implements Command {
    private int playerId;

    public EndTurnCommand(int playerId) {
        this.playerId = playerId;
    }

    public int getPlayerId() {
        return playerId;
    }

    @Override
    public void execute(GameManager game) {
        game.handleAction(this);
    }
}
