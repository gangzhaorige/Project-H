package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.ReadyForSetupCommand;
import com.zzhgl.app.model.Command.ReadyToPlayCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseGameSetup;
import com.zzhgl.app.utility.Log;

import java.util.HashSet;
import java.util.Set;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

/**
 * PreGameState manages the handshake between ChampSelect and actual Gameplay.
 * It waits for clients to load the Game scene and then sends the Setup data.
 */
public class PreGameState implements GameState {
    private enum Phase { WAITING_FOR_SETUP, WAITING_FOR_READY }
    
    private Phase currentPhase = Phase.WAITING_FOR_SETUP;
    private Set<Integer> readyForSetup = new HashSet<>();
    private Set<Integer> readyToPlay = new HashSet<>();
    
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private ScheduledFuture<?> timerFuture;
    private static final int HANDSHAKE_TIMEOUT = 15;

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering PreGameState. Waiting for clients to load scene...");
        startTimer(() -> {
            Log.printf("Setup handshake timed out. Forcing setup data to everyone.");
            sendSetupToAll(game);
        }, HANDSHAKE_TIMEOUT);
    }

    @Override
    public synchronized void handleAction(GameManager game, Command command) {
        if (currentPhase == Phase.WAITING_FOR_SETUP) {
            if (command instanceof ReadyForSetupCommand readyCmd) {
                readyForSetup.add(readyCmd.getPlayerId());
                Log.printf("Player %d is ready for setup data. (%d/%d)", 
                           readyCmd.getPlayerId(), readyForSetup.size(), game.getPlayers().size());
                
                if (readyForSetup.size() >= game.getPlayers().size()) {
                    cancelTimer();
                    sendSetupToAll(game);
                }
            }
        } else if (currentPhase == Phase.WAITING_FOR_READY) {
            if (command instanceof ReadyToPlayCommand readyCmd) {
                readyToPlay.add(readyCmd.getPlayerId());
                Log.printf("Player %d is ready to play. (%d/%d)", 
                           readyCmd.getPlayerId(), readyToPlay.size(), game.getPlayers().size());

                if (readyToPlay.size() >= game.getPlayers().size()) {
                    cancelTimer();
                    startGameplay(game);
                }
            }
        }
    }

    private void sendSetupToAll(GameManager game) {
        currentPhase = Phase.WAITING_FOR_READY;
        Log.printf("Broadcasting ResponseGameSetup to all players.");
        
        ResponseGameSetup setup = new ResponseGameSetup(game.getPlayers());
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(setup);
        }

        startTimer(() -> {
            Log.printf("Ready-to-play handshake timed out. Forcing game start.");
            startGameplay(game);
        }, HANDSHAKE_TIMEOUT);
    }

    private void startGameplay(GameManager game) {
        Log.printf("All players ready or timeout reached. Starting game!");
        game.setState(new GameplayState());
    }

    private void startTimer(Runnable task, int seconds) {
        cancelTimer();
        timerFuture = scheduler.schedule(task, seconds, TimeUnit.SECONDS);
    }

    private void cancelTimer() {
        if (timerFuture != null && !timerFuture.isDone()) {
            timerFuture.cancel(false);
        }
    }

    @Override
    public void onExit(GameManager game) {
        Log.printf("Exiting PreGameState.");
        cancelTimer();
        scheduler.shutdown();
    }

    @Override
    public void onPause(GameManager game) {}

    @Override
    public void onResume(GameManager game) {}
}
