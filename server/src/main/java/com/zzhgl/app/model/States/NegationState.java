package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.PassPriorityCommand;
import com.zzhgl.app.model.Command.PlayCardCommand;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.networking.response.game.ResponsePassPriority;
import com.zzhgl.app.networking.response.game.ResponsePlayCard;
import com.zzhgl.app.networking.response.game.ResponseTimerCancel;
import com.zzhgl.app.networking.response.game.ResponseTimerStart;
import com.zzhgl.app.utility.Log;

import java.util.*;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

public class NegationState implements GameState {

    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private ScheduledFuture<?> timerFuture;
    private static final int NEGATION_WINDOW_SECONDS = 15;
    private final Set<Integer> passedPlayers = new HashSet<>();
    private final Random random = new Random();
    private int lastNegatorId = -1;

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering NegationState. Priority window opened.");
        passedPlayers.clear();
        lastNegatorId = -1;
        startTimer(game);
    }

    private synchronized void startTimer(GameManager game) {
        // Stop any existing timer internally without broadcasting CANCEL to avoid UI flicker
        if (timerFuture != null && !timerFuture.isDone()) {
            timerFuture.cancel(false);
        }
        
        int duration = NEGATION_WINDOW_SECONDS;
        boolean someoneElseHasNegate = anybodyElseHasNegate(game);
        
        if (!someoneElseHasNegate) {
            // Randomly generate 5-7 seconds to hide the fact that no one can counter
            duration = 5 + random.nextInt(3); 
            Log.printf("No one else has Negate. Using hidden random timer: %d seconds.", duration);
        }

        // Notify all players (-1) to pulse indicators as requested
        broadcast(game, new ResponseTimerStart(-1, duration, "Any player can play NEGATE!"));

        timerFuture = scheduler.schedule(() -> {
            Log.printf("Negation window expired. Proceeding to resolution.");
            resolve(game);
        }, duration, TimeUnit.SECONDS);
    }

    private boolean anybodyElseHasNegate(GameManager game) {
        for (Player p : game.getAlivePlayers()) {
            if (p.getID() == lastNegatorId) continue; // Skip the person who just negated
            
            boolean hasNegate = p.getHand().getCards().stream()
                .anyMatch(c -> c instanceof AbstractSpecialCard s && s.getSpecialType() == AbstractSpecialCard.SpecialType.NEGATE);
            if (hasNegate) return true;
        }
        return false;
    }

    private void resolve(GameManager game) {
        game.popState();
        game.resolveStack();
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
        if (command instanceof PlayCardCommand playCmd) {
            handlePlayCard(game, playCmd);
        } else if (command instanceof PassPriorityCommand passCmd) {
            handlePass(game, passCmd);
        }
    }

    private void handlePlayCard(GameManager game, PlayCardCommand playCmd) {
        Player player = game.getPlayerMap().get(playCmd.getPlayerId());
        if (player == null) return;

        Optional<AbstractCard> cardOpt = player.getHand().getCards().stream()
                .filter(c -> c.getId() == playCmd.getCardId())
                .findFirst();

        if (cardOpt.isPresent()) {
            AbstractCard card = cardOpt.get();
            
            if (card instanceof AbstractSpecialCard special && special.getSpecialType() == AbstractSpecialCard.SpecialType.NEGATE) {
                player.getHand().removeCard(card);
                Log.printf("Player %d played NegateCard in NegationState.", player.getID());
                
                card.play(game, player, playCmd.getTargetIds());
                
                broadcast(game, new ResponsePlayCard(player.getID(), card, playCmd.getTargetIds()));

                // Reset: Everyone must be prompted again, except the one who just played it
                passedPlayers.clear();
                passedPlayers.add(player.getID()); 
                lastNegatorId = player.getID();
                startTimer(game);
            } else {
                Log.printf_e("Player %d tried to play non-Negate card in NegationState.", player.getID());
            }
        }
    }

    private void handlePass(GameManager game, PassPriorityCommand passCmd) {
        Log.printf("Player %d passed priority.", passCmd.getPlayerId());
        passedPlayers.add(passCmd.getPlayerId());
        
        Player p = game.getPlayerMap().get(passCmd.getPlayerId());
        if (p != null) {
            p.addResponseForUpdate(new ResponsePassPriority());
        }

        // Check if all alive players have passed
        if (passedPlayers.size() >= game.getAlivePlayers().size()) {
            Log.printf("All alive players passed. Resolving early.");
            cancelTimer(game);
            resolve(game);
        }
    }

    @Override
    public void onExit(GameManager game) {
        Log.printf("Exiting NegationState.");
        cancelTimer(game);
        scheduler.shutdown();
    }

    @Override
    public void onPause(GameManager game) {
        cancelTimer(game);
    }

    @Override
    public void onResume(GameManager game) {
        startTimer(game);
    }
}
