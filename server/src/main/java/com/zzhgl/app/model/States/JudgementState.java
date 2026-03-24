package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.effects.AbstractEffect;
import com.zzhgl.app.networking.response.game.ResponseJudgement;
import com.zzhgl.app.utility.Log;

public class JudgementState implements GameState {
    private final AbstractEffect effect;
    private AbstractCard judgementCard;
    private GameEvent lastEvent;

    public JudgementState(AbstractEffect effect) {
        this.effect = effect;
    }

    public void setJudgementCard(AbstractCard card) {
        this.judgementCard = card;
    }

    public AbstractEffect getEffect() {
        return effect;
    }

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering JudgementState for effect %s on Player %d", effect.getName(), effect.getTarget().getID());

        // 1. Reveal top card
        judgementCard = game.getDrawPile().draw();
        if (judgementCard == null) {
            Log.printf_e("Draw pile empty during Judgement! Reshuffling or failing.");
            // In a real game, you might reshuffle discard pile here. 
            // For now, if null, we just fail the judgement.
            finishJudgement(game);
            return;
        }

        Log.printf("Judgement card drawn: %s", judgementCard);

        // Broadcast current evaluation
        broadcastCurrentResult(game);

        // 2. Emit event for JudgementOverride skills
        lastEvent = new GameEvent(GameEvent.EventType.BEFORE_JUDGEMENT_RESOLVE);
        lastEvent.setData(judgementCard); 
        game.emitEvent(lastEvent);
        
        // If a skill triggers, it will push SkillResolutionState.
        // Once that state pops, it will call onResume here.
        // If no skill triggers, emitEvent does nothing and we just call finishJudgement immediately.
        if (game.getCurrentState() == this) {
            finishJudgement(game);
        }
    }

    private void broadcastCurrentResult(GameManager game) {
        if (judgementCard != null) {
            boolean triggered = effect.evaluateJudgement(game, judgementCard);
            ResponseJudgement res = new ResponseJudgement(judgementCard, triggered);
            for (com.zzhgl.app.model.core.Player p : game.getPlayers()) {
                p.addResponseForUpdate(res);
            }
        }
    }

    private void finishJudgement(GameManager game) {
        // 1. The effect is consumed whether it triggered or not.
        effect.getTarget().removeEffect(effect);

        // 2. Evaluate the condition and queue consequences BEFORE popping
        if (judgementCard != null) {
            boolean triggered = effect.evaluateJudgement(game, judgementCard);
            if (triggered) {
                java.util.List<com.zzhgl.app.model.actions.GameAction> actions = effect.applyConsequence(game, judgementCard);
                if (actions != null) {
                    game.getActionQueue().addAll(actions);
                }
            }

            // 3. Check if card is still "available" (not in any hand) before discarding.
            boolean inAnyHand = false;
            for (com.zzhgl.app.model.core.Player p : game.getPlayers()) {
                if (p.getHand().getCards().contains(judgementCard)) {
                    inAnyHand = true;
                    break;
                }
            }

            if (!inAnyHand) {
                // Put the judgement card in the discard pile
                game.getDiscardPile().addCard(judgementCard);
            }
        }
        game.popState();

        // If actions were added (e.g. AddCardToHand), ensure they are processed with pacing
        if (!game.getActionQueue().isEmpty()) {
            game.pushState(new ActionResolveState());
        }
    }


    @Override
    public void onExit(GameManager game) {
        Log.printf("Exiting JudgementState.");
    }

    @Override
    public void onPause(GameManager game) {
        Log.printf("Pausing JudgementState for potential Override.");
    }

    @Override
    public void onResume(GameManager game) {
        Log.printf("Resuming JudgementState. Checking if card was overridden.");
        
        // Retrieve updated card from event (skills should have updated event.setData())
        if (lastEvent != null && lastEvent.getData() instanceof AbstractCard newCard) {
            this.judgementCard = newCard;
            Log.printf("Judgement card is now: %s", judgementCard);
        }
        
        finishJudgement(game);
    }

    @Override
    public void handleAction(GameManager game, Command command) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'handleAction'");
    }
}
