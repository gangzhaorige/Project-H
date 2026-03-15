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

    @Override
    public void onEnter(GameManager game) {
        // 1. Increment the index first
        int nextIndex = (game.getActivePlayerIndex() + 1) % game.getPlayers().size();
        game.setActivePlayerIndex(nextIndex);
        
        // If we wrapped back to 0, increment the global turn counter
        if (nextIndex == 0) {
            game.setTurnCounter(game.getTurnCounter() + 1);
        }

        // 2. Get the current active player
        Player activePlayer = game.getPlayers().get(nextIndex);
        
        Log.printf("Starting Turn %d for Player %d (%s)", 
                   game.getTurnCounter(), activePlayer.getID(), activePlayer.getUsername());

        // 3. Notify all players whose turn it is
        ResponseTurnStart response = new ResponseTurnStart(activePlayer.getID());
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }

        // 4. Immediately transition to TurnBeginState to start this player's draw phase
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
