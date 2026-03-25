package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.SelectCardsCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseMoveCard;
import com.zzhgl.app.networking.response.game.ResponseSelectCards;
import com.zzhgl.app.networking.response.game.ResponseSelectCardsFromOpponent;
import com.zzhgl.app.networking.response.game.ResponseTimerStart;
import com.zzhgl.app.utility.Log;

import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

public class StealState implements GameState {
    private final Player caster;
    private final Player target;
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private ScheduledFuture<?> timerFuture;
    private static final int DURATION = 15;

    public StealState(Player caster, Player target) {
        this.caster = caster;
        this.target = target;
    }

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Player %d is choosing card to steal from %d.", caster.getID(), target.getID());

        int handSize = target.getHand().size();
        ResponseSelectCardsFromOpponent response = new ResponseSelectCardsFromOpponent(target.getID(), 1, DURATION, "Choose 1 card to steal.", handSize);
        caster.addResponseForUpdate(response);

        // 2. Start timer
        timerFuture = scheduler.schedule(() -> {
            Log.printf("Steal selection timeout for player %d.", caster.getID());
            game.popState();
        }, DURATION, TimeUnit.SECONDS);

        // 3. Notify all about the timer
        ResponseTimerStart timerRes = new ResponseTimerStart(caster.getID(), DURATION, "Stealing...", "ANY");
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(timerRes);
        }
    }

    @Override
    public void handleAction(GameManager game, Command command) {
        if (command instanceof SelectCardsCommand cmd && cmd.getPlayerId() == caster.getID()) {
            if (cmd.getCardIndices().size() == 1) {
                cancelTimer();
                
                // Process the steal
                int cardIndex = cmd.getCardIndices().get(0);
                if (cardIndex >= 0 && cardIndex < target.getHand().getCards().size()) {
                    var stolenCard = target.getHand().getCards().get(cardIndex);
                    target.getHand().removeCard(stolenCard);
                    caster.getHand().addCard(stolenCard);
                    Log.printf("Player %d stole %s.", caster.getID(), stolenCard);

                    // Notify caster to close UI
                    caster.addResponseForUpdate(new ResponseSelectCards());

                    // Broadcast card movement
                    List<com.zzhgl.app.model.cards.AbstractCard> cards = List.of(stolenCard);
                    ResponseMoveCard detailedRes = new ResponseMoveCard(cards, caster.getID(), target.getID(), true);
                    ResponseMoveCard hiddenRes = new ResponseMoveCard(cards, caster.getID(), target.getID(), false);

                    for (Player p : game.getPlayers()) {
                        if (p.getID() == caster.getID() || p.getID() == target.getID()) {
                            p.addResponseForUpdate(detailedRes);
                        } else {
                            p.addResponseForUpdate(hiddenRes);
                        }
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
