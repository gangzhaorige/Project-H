package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.PassPriorityCommand;
import com.zzhgl.app.model.Command.PlayCardCommand;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.interactions.DamageInteraction;
import com.zzhgl.app.networking.response.game.ResponsePassPriority;
import com.zzhgl.app.networking.response.game.ResponsePlayCard;
import com.zzhgl.app.networking.response.game.ResponseTimerCancel;
import com.zzhgl.app.networking.response.game.ResponseTimerStart;
import com.zzhgl.app.utility.Log;
import com.zzhgl.app.model.cards.AbstractNormalCard.NormalType;

import java.util.Optional;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

public class DuelPromptState implements GameState {
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private ScheduledFuture<?> timerFuture;
    private static final int WINDOW_SECONDS = 15;

    private Player currentResponder;
    private Player otherPlayer;
    private AbstractInteraction sourceInteraction;
    private int damageAmount;

    public DuelPromptState(Player target, Player caster, AbstractInteraction sourceInteraction, int damageAmount) {
        // Target must respond first
        this.currentResponder = target;
        this.otherPlayer = caster;
        this.sourceInteraction = sourceInteraction;
        this.damageAmount = damageAmount;
    }

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering DuelPromptState. %s must play ATTACK.", currentResponder.getUsername());
        startTimer(game);
    }

    private synchronized void startTimer(GameManager game) {
        cancelTimer();
        
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(new ResponseTimerStart(currentResponder.getID(), WINDOW_SECONDS, "Play ATTACK to continue Duel!", NormalType.ATTACK));
        }

        timerFuture = scheduler.schedule(() -> {
            Log.printf("Duel window expired for player %d. Taking damage.", currentResponder.getID());
            applyDamage(game, otherPlayer, currentResponder);
            game.popState();
        }, WINDOW_SECONDS, TimeUnit.SECONDS);
    }

    private void applyDamage(GameManager game, Player source, Player target) {
        DamageInteraction dmg = new DamageInteraction(source, target, sourceInteraction.getCard(), damageAmount);
        game.getInteractionStack().push(dmg);
    }

    private synchronized void cancelTimer() {
        if (timerFuture != null && !timerFuture.isDone()) {
            timerFuture.cancel(false);
        }
    }

    @Override
    public synchronized void handleAction(GameManager game, Command command) {
        if (command instanceof PlayCardCommand playCmd) {
            if (playCmd.getPlayerId() != currentResponder.getID()) {
                Log.printf_e("Player %d tried to play a card, but it is Player %d's turn in Duel.", playCmd.getPlayerId(), currentResponder.getID());
                return;
            }

            Player player = game.getPlayerMap().get(playCmd.getPlayerId());
            Optional<AbstractCard> cardOpt = player.getHand().getCards().stream()
                    .filter(c -> c.getId() == playCmd.getCardId())
                    .findFirst();

            if (cardOpt.isPresent()) {
                AbstractCard card = cardOpt.get();
                
                if (card instanceof AbstractNormalCard normal && normal.getNormalType() == AbstractNormalCard.NormalType.ATTACK) {
                    player.getHand().removeCard(card);
                    Log.printf("Player %d continued duel with %s.", player.getID(), card);
                    
                    // Notify clients
                    ResponsePlayCard response = new ResponsePlayCard(player.getID(), card, playCmd.getTargetIds());
                    for (Player p : game.getPlayers()) p.addResponseForUpdate(response);

                    // Swap responder and restart timer
                    Player temp = currentResponder;
                    currentResponder = otherPlayer;
                    otherPlayer = temp;
                    startTimer(game);
                } else {
                    Log.printf_e("Player %d must play an Attack card in a duel.", player.getID());
                }
            }
        } else if (command instanceof PassPriorityCommand passCmd) {
            if (passCmd.getPlayerId() != currentResponder.getID()) {
                Log.printf_e("Player %d tried to pass, but it is Player %d's turn in Duel.", passCmd.getPlayerId(), currentResponder.getID());
                return;
            }
            Log.printf("Player %d chose to pass the duel. Taking damage.", currentResponder.getID());
            
            currentResponder.addResponseForUpdate(new ResponsePassPriority());

            cancelTimer();
            applyDamage(game, otherPlayer, currentResponder);
            game.popState();
        }
    }

    @Override
    public void onExit(GameManager game) {
        cancelTimer();
        for (Player p : game.getPlayers()) p.addResponseForUpdate(new ResponseTimerCancel());
        scheduler.shutdown();
    }

    @Override
    public void onPause(GameManager game) {
        cancelTimer();
    }

    @Override
    public void onResume(GameManager game) {
        startTimer(game);
    }
}
