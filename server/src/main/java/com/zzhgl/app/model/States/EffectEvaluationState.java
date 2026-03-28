package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.effects.AbstractEffect;
import com.zzhgl.app.model.interactions.types.EffectActivationInteraction;
import com.zzhgl.app.utility.Log;

import java.util.LinkedList;
import java.util.Queue;
import java.util.List;

public class EffectEvaluationState implements GameState {
    private Queue<AbstractEffect> pendingEffects = new LinkedList<>();

    @Override
    public void onEnter(GameManager game) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        
        // Copy the list to avoid concurrent modification issues, and evaluate in order
        List<AbstractEffect> effects = activePlayer.getActiveEffects();
        if (effects.isEmpty()) {
            // No effects, proceed to normal turn start
            game.setState(new TurnBeginState());
            return;
        }

        Log.printf("Player %d has %d active effects to evaluate.", activePlayer.getID(), effects.size());
        pendingEffects.addAll(effects);
        
        evaluateNext(game);
    }

    private void evaluateNext(GameManager game) {
        if (pendingEffects.isEmpty()) {
            Log.printf("All effects evaluated. Moving to TurnBeginState.");
            game.setState(new TurnBeginState());
            return;
        }

        AbstractEffect currentEffect = pendingEffects.poll();
        Log.printf("Evaluating effect: %s", currentEffect.getName());

        // We push an EffectActivationInteraction. 
        // If it's negatable, game.resolveStack() will naturally push NegationState.
        // If it's not negated, it resolves into JudgementState.
        // If it IS negated, the interaction is destroyed, and the effect doesn't happen.
        // EITHER WAY, once that nested resolution finishes, it pops back to here, 
        // and we must ensure we continue to the next effect in `onResume`.
        
        game.getInteractionStack().push(new EffectActivationInteraction(currentEffect));
        game.resolveStack();
    }

    @Override
    public void handleAction(GameManager game, Command command) {}

    @Override
    public void onExit(GameManager game) {}

    @Override
    public void onPause(GameManager game) {}

    @Override
    public void onResume(GameManager game) {
        // A child state (like NegationState -> JudgementState -> ActionResolveState) finished.
        
        // --- NEW: Check if there are actions pending from that resolution ---
        game.resolveActions();
        if (game.getCurrentState() != this) return;

        // Evaluate the next effect in the queue.
        evaluateNext(game);
    }
}
