package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.utility.Log;

/**
 * Action to set the extra draw count for the current turn.
 */
public class SetExtraDrawAction implements GameAction {
    private final int bonus;

    public SetExtraDrawAction(int bonus) {
        this.bonus = bonus;
    }

    @Override
    public void execute(GameManager game) {
        Log.printf("SetExtraDrawAction: Adding %d to extra draw count (Previous: %d)", bonus, game.getExtraDrawCount());
        game.setExtraDrawCount(game.getExtraDrawCount() + bonus);
    }
}
