package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.EndTurnCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

/**
 * PlayActionState is where the active player can play cards.
 */
public class PlayActionState implements GameState {
    @Override
    public void onEnter(GameManager game) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        Log.printf("Entering PlayActionState for player %d (%s).", 
                   activePlayer.getID(), activePlayer.getUsername());
    }

    @Override
    public synchronized void handleAction(GameManager game, Command command) {
        if (command instanceof EndTurnCommand endCmd) {
            Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
            if (endCmd.getPlayerId() == activePlayer.getID()) {
                Log.printf("Player %d is ending their turn.", activePlayer.getID());
                game.setState(new TurnEndState());
            }
        }
        // Handle playing cards here later
    }

    @Override
    public void onExit(GameManager game) {}

    @Override
    public void onPause(GameManager game) {}

    @Override
    public void onResume(GameManager game) {}
}
