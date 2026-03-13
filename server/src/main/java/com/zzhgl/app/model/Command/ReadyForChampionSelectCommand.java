package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;

public class ReadyForChampionSelectCommand implements Command {
    private int playerId;

    public ReadyForChampionSelectCommand(int playerId) {
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
