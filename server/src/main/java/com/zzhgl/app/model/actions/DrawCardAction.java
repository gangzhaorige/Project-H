package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;

public class DrawCardAction implements GameAction {
    private final Player player;
    private final int count;

    public DrawCardAction(Player player, int count) {
        this.player = player;
        this.count = count;
    }

    @Override
    public void execute(GameManager game) {
        game.drawCards(player, count);
    }
}
