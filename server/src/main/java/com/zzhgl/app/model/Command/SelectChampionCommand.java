package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;

public class SelectChampionCommand implements Command {
    private int playerId;
    private int championId;

    public SelectChampionCommand(int playerId, int championId) {
        this.playerId = playerId;
        this.championId = championId;
    }

    public int getPlayerId() {
        return playerId;
    }

    public int getChampionId() {
        return championId;
    }

    @Override
    public void execute(GameManager game) {
        game.handleAction(this);
    }
}
