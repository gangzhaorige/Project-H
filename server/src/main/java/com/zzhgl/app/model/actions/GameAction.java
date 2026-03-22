package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.core.GameManager;

/**
 * GameAction represents a single atomic operation in the game logic.
 * Skills return a sequence of these actions to be executed by the state machine.
 */
public interface GameAction {
    /**
     * Executes the action.
     * @param game The game manager.
     */
    void execute(GameManager game);

    /**
     * Returns true if this action "blocks" the sequence (e.g., by pushing a state).
     * If true, the action processor will wait for the pushed state to pop before continuing.
     */
    default boolean isBlocking() {
        return false;
    }
}
