package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.utility.Log;

import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

/**
 * ActionResolveState processes the GameManager's ActionQueue one by one
 * with a fixed 1-second interval between actions.
 */
public class ActionResolveState implements GameState {
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private boolean isPaused = false;

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering ActionResolveState. Processing queue of size: %d", game.getActionQueue().size());
        processNext(game);
    }

    private synchronized void processNext(GameManager game) {
        if (isPaused) return;

        if (game.getActionQueue().isEmpty()) {
            Log.printf("ActionQueue empty. Popping ActionResolveState.");
            game.popState();
            return;
        }

        GameAction action = game.getActionQueue().poll();
        Log.printf("ActionResolveState: Executing %s", action.getClass().getSimpleName());
        action.execute(game);

        // If the action pushed a new state (like JudgementState or a Delay), 
        // we will be paused. onResume will handle the next step.
        if (action.isBlocking() || game.getCurrentState() != this) {
            Log.printf("Action %s is blocking or pushed a state. Pausing sequence.", action.getClass().getSimpleName());
            return;
        }

        // Otherwise, wait 1 second and then process the next action
        scheduleNext(game);
    }

    private void scheduleNext(GameManager game) {
        scheduler.schedule(() -> {
            processNext(game);
        }, 1, TimeUnit.SECONDS);
    }

    @Override
    public void onResume(GameManager game) {
        Log.printf("ActionResolveState resumed. Waiting 1s before next action...");
        isPaused = false;
        // After a sub-state finishes, we still want the 1s interval 
        // before the next item in the queue.
        scheduleNext(game);
    }

    @Override
    public void onPause(GameManager game) {
        isPaused = true;
    }

    @Override
    public void onExit(GameManager game) {
        scheduler.shutdown();
    }

    @Override
    public void handleAction(GameManager game, Command command) {}
}
