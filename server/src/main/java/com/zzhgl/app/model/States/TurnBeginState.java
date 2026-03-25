package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseChampionStatsUpdateInteger;
import com.zzhgl.app.utility.Log;

/**
 * TurnBeginState handles the "Draw Phase" of a player's turn.
 */
public class TurnBeginState implements GameState {
    @Override
    public void onEnter(GameManager game) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        Log.printf("Player %d turn begin.", activePlayer.getID());

        // Reset attack count
        if (activePlayer.getSelectedChampion() != null) {
            activePlayer.getSelectedChampion().setCurNumOfAttack(0);
            
            // Notify all players about the reset
            ResponseChampionStatsUpdateInteger statsUpdate = new ResponseChampionStatsUpdateInteger(
                activePlayer.getSelectedChampion().getId(),
                Champion.STAT_CUR_NUM_ATTACK,
                0
            );
            for (Player p : game.getPlayers()) {
                p.addResponseForUpdate(statsUpdate);
            }
        }

        // Emit TURN_BEGIN event. All reactive skills (like Beginning Wisdom) will be queued.
        game.emitEvent(new GameEvent(GameEvent.EventType.TURN_BEGIN));

        // If a skill (like Seele's MyTurn) was triggered, it pushes SkillResolutionState.
        // We wait until it's finished and we resume.
        if (game.getCurrentState() == this) {
            proceedToDraw(game);
        }
    }

    private void proceedToDraw(GameManager game) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        
        // Draw cards only if not skipped by a skill
        if (!game.isSkipDrawPhase()) {
            int count = 2 + game.getExtraDrawCount();
            Log.printf("Player %d is drawing %d cards (2 + %d bonus).", activePlayer.getID(), count, game.getExtraDrawCount());
            game.drawCards(activePlayer, count);
        } else {
            Log.printf("Player %d skipped draw phase via skill.", activePlayer.getID());
            game.setSkipDrawPhase(false); // Reset flag
        }

        // Reset extra draw count after drawing
        game.setExtraDrawCount(0);

        // Check if a Judgement (like Prison) requested to skip the action phase
        if (game.isSkipActionPhase()) {
            Log.printf("Action phase skipped for Player %d due to active effects.", activePlayer.getID());
            game.setSkipActionPhase(false); // Reset flag
            game.setState(new TurnEndState());
        } else {
            // Move to PlayActionState
            game.setState(new PlayActionState());
        }
    }

    @Override
    public void handleAction(GameManager game, Command command) {}

    @Override
    public void onExit(GameManager game) {}

    @Override
    public void onPause(GameManager game) {
        Log.printf("TurnBeginState paused.");
    }

    @Override
    public void onResume(GameManager game) {
        Log.printf("TurnBeginState resumed.");
        proceedToDraw(game);
    }
}
