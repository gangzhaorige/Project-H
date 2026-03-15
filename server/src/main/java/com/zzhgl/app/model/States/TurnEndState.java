package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseEndTurn;
import com.zzhgl.app.utility.Log;

/**
 * TurnEndState handles end-of-turn cleanup and notifications.
 */
public class TurnEndState implements GameState {
    @Override
    public void onEnter(GameManager game) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        Log.printf("Player %d turn ended.", activePlayer.getID());

        // Notify all players
        ResponseEndTurn response = new ResponseEndTurn(activePlayer.getID());
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }

        // Return to TurnState to start the next player's turn
        game.setState(new TurnState());
    }

    @Override
    public void handleAction(GameManager game, Command command) {}

    @Override
    public void onExit(GameManager game) {}

    @Override
    public void onPause(GameManager game) {}

    @Override
    public void onResume(GameManager game) {}
}
