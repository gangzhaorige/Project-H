package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.utility.Log;

import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

/**
 * DelayState is a non-blocking state that simply waits for a specified duration
 * before popping itself from the state stack.
 */
public class DelayState implements GameState {
    private final int milliseconds;
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();

    public DelayState(int milliseconds) {
        this.milliseconds = milliseconds;
    }

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering DelayState: waiting for %d ms...", milliseconds);
        
        scheduler.schedule(() -> {
            Log.printf("DelayState finished (%d ms). Popping...", milliseconds);
            game.popState();
        }, milliseconds, TimeUnit.MILLISECONDS);
    }

    @Override
    public void onExit(GameManager game) {
        scheduler.shutdown();
    }

    @Override
    public void handleAction(GameManager game, Command command) {
        // Ignore commands during delay
    }

    @Override
    public void onPause(GameManager game) {}

    @Override
    public void onResume(GameManager game) {
        // If we resumed (something pushed on top of delay), we just pop immediately
        // as the timer likely finished while we were paused.
        game.popState();
    }
}
