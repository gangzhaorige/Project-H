package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.interactions.AbstractInteraction;

import java.util.LinkedList;
import java.util.List;
import java.util.Queue;

/**
 * InteractionResolutionState processes a list of game interactions sequentially.
 * This ensures that if an interaction triggers a skill, that skill is fully 
 * resolved before the next interaction starts.
 */
public class InteractionResolutionState implements GameState {
    private final Queue<AbstractInteraction> interactions = new LinkedList<>();

    public InteractionResolutionState(List<AbstractInteraction> interactionList) {
        this.interactions.addAll(interactionList);
    }

    @Override
    public void onEnter(GameManager game) {
        processNext(game);
    }

    @Override
    public void onResume(GameManager game) {
        // This is called after a skill pushed on top of this state is popped.
        processNext(game);
    }

    private void processNext(GameManager game) {
        if (interactions.isEmpty()) {
            game.popState();
            return;
        }

        AbstractInteraction next = interactions.poll();
        next.execute(game);
        
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
