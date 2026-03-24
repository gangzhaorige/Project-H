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

import java.util.Optional;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

public class DefendPromptState implements GameState {
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private ScheduledFuture<?> timerFuture;
    private static final int WINDOW_SECONDS = 15;

    private Player defender;
    private Player attacker;
    private AbstractInteraction sourceInteraction;
    private int damageAmount;
    private AbstractNormalCard.NormalType requiredType;

    public DefendPromptState(Player defender, Player attacker, AbstractInteraction sourceInteraction, int damageAmount, AbstractNormalCard.NormalType requiredType) {
        this.defender = defender;
        this.attacker = attacker;
        this.sourceInteraction = sourceInteraction;
        this.damageAmount = damageAmount;
        this.requiredType = requiredType;
    }

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering DefendPromptState. Player %d must play %s to avoid %d damage.", defender.getID(), requiredType, damageAmount);
        startTimer(game);
    }

    private synchronized void startTimer(GameManager game) {
        cancelTimer();
        
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(new ResponseTimerStart(defender.getID(), WINDOW_SECONDS, "Play " + requiredType + " to defend!", requiredType));
        }

        timerFuture = scheduler.schedule(() -> {
            Log.printf("Defend window expired for player %d. Taking damage.", defender.getID());
            applyDamage(game);
            game.popState();
            // Important: we don't resolveStack here, we just pop. 
            // The InteractionResolutionState is below us and will resume and process next.
        }, WINDOW_SECONDS, TimeUnit.SECONDS);
    }

    private void applyDamage(GameManager game) {
        DamageInteraction dmg = new DamageInteraction(attacker, defender, sourceInteraction.getCard(), damageAmount);
        
        // Add context for skills like Yanqing's passive
        dmg.setExtraParam("sourceInteractionType", sourceInteraction.getClass().getSimpleName());
        dmg.setExtraParam("requiredType", requiredType);
        
        // We push to the stack so it resolves properly through modifiers
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
            if (playCmd.getPlayerId() != defender.getID()) {
                Log.printf_e("Player %d tried to play a card, but it is Player %d's turn to defend.", playCmd.getPlayerId(), defender.getID());
                return;
            }

            Player player = game.getPlayerMap().get(playCmd.getPlayerId());
            Optional<AbstractCard> cardOpt = player.getHand().getCards().stream()
                    .filter(c -> c.getId() == playCmd.getCardId())
                    .findFirst();

            if (cardOpt.isPresent()) {
                AbstractCard card = cardOpt.get();
                
                if (card instanceof AbstractNormalCard normal && normal.getNormalType() == requiredType) {
                    player.getHand().removeCard(card);
                    Log.printf("Player %d successfully defended with %s.", player.getID(), card);
                    
                    // Notify clients
                    ResponsePlayCard response = new ResponsePlayCard(player.getID(), card, playCmd.getTargetIds());
                    for (Player p : game.getPlayers()) p.addResponseForUpdate(response);

                    // They successfully defended, so we just pop state without applying damage
                    cancelTimer();
                    game.popState();
                } else {
                    Log.printf_e("Player %d tried to play the wrong card type to defend.", player.getID());
                }
            }
        } else if (command instanceof PassPriorityCommand passCmd) {
            if (passCmd.getPlayerId() != defender.getID()) {
                Log.printf_e("Player %d tried to pass, but it is Player %d's turn to defend.", passCmd.getPlayerId(), defender.getID());
                return;
            }
            Log.printf("Player %d chose to pass. Taking damage.", defender.getID());
            
            defender.addResponseForUpdate(new ResponsePassPriority());

            cancelTimer();
            applyDamage(game);
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
