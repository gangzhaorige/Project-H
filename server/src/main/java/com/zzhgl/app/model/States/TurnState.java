package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseTurnStart;
import com.zzhgl.app.utility.Log;

/**
 * TurnState updates the index of the active player and transitions to TurnBeginState.
 */
public class TurnState implements GameState {
    private boolean initialized = false;

    @Override
    public void onEnter(GameManager game) {
        // If it's the very first turn, the index is already 0.
        // Otherwise, move to the next player.
        if (initialized) {
            int nextIndex = (game.getActivePlayerIndex() + 1) % game.getPlayers().size();
            game.setActivePlayerIndex(nextIndex);
            
            // If we wrapped back to 0, increment global turn counter
            if (nextIndex == 0) {
                game.setTurnCounter(game.getTurnCounter() + 1);
            }
        }
        initialized = true;

        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        Log.printf("Starting Turn %d for Player %d (%s)", 
                   game.getTurnCounter(), activePlayer.getID(), activePlayer.getUsername());

        // Notify all players
        ResponseTurnStart response = new ResponseTurnStart(activePlayer.getID());
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }

        // Immediately transition to TurnBeginState
        game.setState(new TurnBeginState());
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
