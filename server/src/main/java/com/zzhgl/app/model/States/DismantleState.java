package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.SelectCardsCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseDiscardCard;
import com.zzhgl.app.networking.response.game.ResponseSelectCards;
import com.zzhgl.app.networking.response.game.ResponseSelectCardsFromOpponent;
import com.zzhgl.app.networking.response.game.ResponseTimerStart;
import com.zzhgl.app.utility.Log;

import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

public class DismantleState implements GameState {
    private final Player caster;
    private final Player target;
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private ScheduledFuture<?> timerFuture;
    private static final int DURATION = 15;

    public DismantleState(Player caster, Player target) {
        this.caster = caster;
        this.target = target;
    }

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Player %d is choosing card to dismantle from %d.", caster.getID(), target.getID());

        int handSize = target.getHand().size();
        // Notify caster to select cards from target's hand
        ResponseSelectCardsFromOpponent response = new ResponseSelectCardsFromOpponent(target.getID(), 1, DURATION, "Choose 1 card to dismantle.", handSize);
        caster.addResponseForUpdate(response);

        // 2. Start timer
        timerFuture = scheduler.schedule(() -> {
            Log.printf("Dismantle selection timeout for player %d.", caster.getID());
            game.popState();
        }, DURATION, TimeUnit.SECONDS);

        // 3. Notify all about the timer
        ResponseTimerStart timerRes = new ResponseTimerStart(caster.getID(), DURATION, "Dismantling...", "ANY");
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(timerRes);
        }
    }

    @Override
    public void handleAction(GameManager game, Command command) {
        if (command instanceof SelectCardsCommand cmd && cmd.getPlayerId() == caster.getID()) {
            if (cmd.getCardIndices().size() == 1) {
                cancelTimer();
                
                // Process the dismantle
                int cardIndex = cmd.getCardIndices().get(0);
                if (cardIndex >= 0 && cardIndex < target.getHand().getCards().size()) {
                    var cardToDismantle = target.getHand().getCards().get(cardIndex);
                    target.getHand().removeCard(cardToDismantle);
                    game.getDiscardPile().addCard(cardToDismantle);
                    
                    Log.printf("Player %d dismantled %s from player %d.", caster.getID(), cardToDismantle, target.getID());

                    // Notify caster to close UI
                    caster.addResponseForUpdate(new ResponseSelectCards());

                    // Broadcast dismantle (using ResponseDiscardCard)
                    ResponseDiscardCard discardRes = new ResponseDiscardCard(target.getID(), cardToDismantle, false, false);
                    for (Player p : game.getPlayers()) {
                        p.addResponseForUpdate(discardRes);
                    }
                }

                game.popState();
            }
        }
    }

    private void cancelTimer() {
        if (timerFuture != null && !timerFuture.isDone()) {
            timerFuture.cancel(false);
        }
        scheduler.shutdown();
    }

    @Override
    public void onExit(GameManager game) {
        cancelTimer();
    }

    @Override
    public void onPause(GameManager game) {}
    @Override
    public void onResume(GameManager game) {}
}
