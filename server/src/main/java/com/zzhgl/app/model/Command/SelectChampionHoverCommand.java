package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;

public class SelectChampionHoverCommand implements Command {
    private int playerId;
    private int championId;

    public SelectChampionHoverCommand(int playerId, int championId) {
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
