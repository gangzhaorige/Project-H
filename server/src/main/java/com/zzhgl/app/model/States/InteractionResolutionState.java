package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.utility.Log;

/**
 * InteractionResolutionState processes interactions from the GameStack sequentially.
 * This ensures that if an interaction triggers a skill, that skill is fully 
 * resolved before the next interaction starts.
 */
public class InteractionResolutionState implements GameState {

    public InteractionResolutionState() {
    }

    @Override
    public void onEnter(GameManager game) {
        processNext(game);
    }

    @Override
    public void onResume(GameManager game) {
        // This is called after a skill/judgement/resolve pushed on top of this state is popped.
        
        // --- NEW: Check if there are actions pending from that resolution ---
        game.resolveActions();
        if (game.getCurrentState() != this) return;

        processNext(game);
    }

    private void processNext(GameManager game) {
        if (game.getInteractionStack().isEmpty()) {
            game.popState();
            return;
        }

        AbstractInteraction next = game.getInteractionStack().peek();
        if (next.isCanceled()) {
            game.getInteractionStack().pop();
            Log.printf("Skipping canceled interaction from %s.", next.getCaster().getUsername());
            processNext(game);
            return;
        }

        // --- NEW: Centralized Negation Window ---
        if (next.isNegatable() && !next.isNegationEvaluated()) {
            Log.printf("Interaction from %s is negatable. Pushing NegationState.", next.getCaster().getUsername());
            next.setNegationEvaluated(true);
            game.pushState(new NegationState(next));
            return;
        }
        // ----------------------------------------

        // Actually pop it now that we are resolving its evaluation
        game.getInteractionStack().pop();
        next.evaluate(game);
        
        // If the execution didn't push a new state (like SkillResolutionState),
        // we continue to the next interaction immediately.
        // If it DID push a state, this state is now paused, and onResume will call processNext.
        if (game.getCurrentState() == this) {
            processNext(game);
        }
    }

    @Override
    public void handleAction(GameManager game, Command command) {}

    @Override
    public void onExit(GameManager game) {}

    @Override
    public void onPause(GameManager game) {}
}
