package com.zzhgl.app.model.States;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.Command.Command;

/**
 * GameState interface for the state machine handling game logic.
 */
public interface GameState {
    void onEnter(GameManager game);
    void handleAction(GameManager game, Command command);
    void onExit(GameManager game);
    void onPause(GameManager game);
    void onResume(GameManager game);
}
