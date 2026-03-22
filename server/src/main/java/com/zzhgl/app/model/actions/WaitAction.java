package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.States.DelayState;
import com.zzhgl.app.model.core.GameManager;

/**
 * WaitAction initiates a non-blocking delay.
 */
public class WaitAction implements GameAction {
    private final int milliseconds;

    public WaitAction(int milliseconds) {
        this.milliseconds = milliseconds;
    }

    @Override
    public void execute(GameManager game) {
        // Push the DelayState which uses a scheduler to pop itself later.
        game.pushState(new DelayState(milliseconds));
    }

    @Override
    public boolean isBlocking() {
        // This is a blocking action because we want the queue to wait until 
        // DelayState pops before running the next action.
        return true;
    }
}
