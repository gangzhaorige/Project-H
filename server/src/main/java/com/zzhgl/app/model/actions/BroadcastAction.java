package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.GameResponse;

/**
 * BroadcastAction sends a response to all players in the game.
 */
public class BroadcastAction implements GameAction {
    private final GameResponse response;

    public BroadcastAction(GameResponse response) {
        this.response = response;
    }

    @Override
    public void execute(GameManager game) {
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }
    }
}
