package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.utility.Log;

/**
 * GameplayState is the main state where the actual card game happens.
 */
public class GameplayState implements GameState {
    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering GameplayState. Starting first turn.");
        // Logic to start the first player's turn
    }

    @Override
    public void handleAction(GameManager game, Command command) {
        // Handle in-game actions like playing cards, attacking, etc.
    }

    @Override
    public void onExit(GameManager game) {
        Log.printf("Exiting GameplayState.");
    }

    @Override
    public void onPause(GameManager game) {}

    @Override
    public void onResume(GameManager game) {}
}
