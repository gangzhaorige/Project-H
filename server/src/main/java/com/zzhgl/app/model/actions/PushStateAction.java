package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.States.GameState;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.utility.Log;

/**
 * PushStateAction pushes a new GameState to the state stack.
 */
public class PushStateAction implements GameAction {
    private final GameState state;

    public PushStateAction(GameState state) {
        this.state = state;
    }

    @Override
    public void execute(GameManager game) {
        Log.printf("PushStateAction: pushing %s", state.getClass().getSimpleName());
        game.pushState(state);
    }

    @Override
    public boolean isBlocking() {
        // This is blocking because we want the queue to pause until this state finishes.
        return true;
    }
}
