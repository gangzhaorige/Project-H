package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

/**
 * TurnBeginState handles the "Draw Phase" of a player's turn.
 */
public class TurnBeginState implements GameState {
    @Override
    public void onEnter(GameManager game) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        Log.printf("Player %d is drawing cards.", activePlayer.getID());

        // Reset attack count
        if (activePlayer.getSelectedChampion() != null) {
            activePlayer.getSelectedChampion().setCurNumOfAttack(0);
        }

        // Emit TURN_BEGIN event. All reactive skills (like Beginning Wisdom) will be queued.
        game.emitEvent(new GameEvent(GameEvent.EventType.TURN_BEGIN));

        // Default: Draw 2 cards
        game.drawCards(activePlayer, 2);

        // Move to PlayActionState
        game.setState(new PlayActionState());
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
