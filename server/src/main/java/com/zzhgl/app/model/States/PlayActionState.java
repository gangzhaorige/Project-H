package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.EndTurnCommand;
import com.zzhgl.app.model.Command.PlayCardCommand;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.networking.response.game.ResponseGameState;
import com.zzhgl.app.networking.response.game.ResponsePlayCard;
import com.zzhgl.app.networking.response.game.ResponseTimerCancel;
import com.zzhgl.app.networking.response.game.ResponseTimerStart;
import com.zzhgl.app.utility.Log;

import java.util.List;
import java.util.Optional;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

/**
 * PlayActionState is where the active player can play cards.
 */
public class PlayActionState implements GameState {
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private ScheduledFuture<?> timerFuture;
    private static final int TIMEOUT_SECONDS = 15;

    @Override
    public void onEnter(GameManager game) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        Log.printf("Entering PlayActionState for player %d (%s).", 
                   activePlayer.getID(), activePlayer.getUsername());
        
        startTimer(game);
    }

    private synchronized void startTimer(GameManager game) {
        cancelTimer(game);
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        broadcast(game, new ResponseTimerStart(activePlayer.getID(), TIMEOUT_SECONDS, "Your turn to play cards!", "ANY"));


        timerFuture = scheduler.schedule(() -> {
            Log.printf("Player timeout. Ending turn.");
            game.setState(new TurnEndState());
        }, TIMEOUT_SECONDS, TimeUnit.SECONDS);
    }

    private synchronized void cancelTimer(GameManager game) {
        if (timerFuture != null && !timerFuture.isDone()) {
            timerFuture.cancel(false);
            broadcast(game, new ResponseTimerCancel());
        }
    }

    private void broadcast(GameManager game, GameResponse response) {
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }
    }

    @Override
    public synchronized void handleAction(GameManager game, Command command) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());

        if (command instanceof EndTurnCommand endCmd) {
            if (endCmd.getPlayerId() == activePlayer.getID()) {
                Log.printf("Player %d is ending their turn.", activePlayer.getID());
                cancelTimer(game);
                game.setState(new TurnEndState());
            }
        } 
        else if (command instanceof PlayCardCommand playCmd) {
            if (playCmd.getPlayerId() == activePlayer.getID()) {
                handlePlayCard(game, activePlayer, playCmd);
            }
        }
    }

    private void handlePlayCard(GameManager game, Player player, PlayCardCommand cmd) {
        // 1. Find the card in the hand
        Optional<AbstractCard> cardOpt = player.getHand().getCards().stream()
                .filter(c -> c.getId() == cmd.getCardId())
                .findFirst();

        if (cardOpt.isPresent()) {
            AbstractCard card = cardOpt.get();
            
            // Validate if the card can be played
            if (!card.validate(game, player, cmd.getTargetIds())) {
                Log.printf_e("Player %d tried to play card %s (ID: %d) but validation failed!", player.getID(), card.getClass().getSimpleName(), card.getId());
                return; // Ignore the request
            }
            
            // 2. Remove from hand
            player.getHand().removeCard(card);
            
            Log.printf("Player %d played card: %s (ID: %d) against targets: %s", 
                       player.getID(), card, card.getId(), cmd.getTargetIds());

            // 3. Play the card, pushing interactions to the stack
            card.play(game, player, cmd.getTargetIds());
            game.emitEvent(new GameEvent(GameEvent.EventType.CARD_PLAYED).setParam("card", card));
            // 4. Notify everyone
            ResponsePlayCard response = new ResponsePlayCard(player.getID(), card, cmd.getTargetIds());
            for (Player p : game.getPlayers()) {
                p.addResponseForUpdate(response);
            }

            // 5. Evaluate the stack
            if (!game.getInteractionStack().isEmpty()) {
                game.resolveStack();
            }
        } else {
            Log.printf_e("Player %d tried to play card %d but it's not in their hand!", 
                         player.getID(), cmd.getCardId());
        }
    }

    @Override
    public void onExit(GameManager game) {
        Log.printf("Exiting PlayActionState.");
        cancelTimer(game);
        scheduler.shutdown();
    }

    @Override
    public void onPause(GameManager game) {
        Log.printf("Pausing PlayActionState.");
        cancelTimer(game);
    }

    @Override
    public void onResume(GameManager game) {
        Log.printf("Resuming PlayActionState.");
        startTimer(game);
    }
}
