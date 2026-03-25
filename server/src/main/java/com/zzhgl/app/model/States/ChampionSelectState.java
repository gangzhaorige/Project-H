package com.zzhgl.app.model.States;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.SelectChampionCommand;
import com.zzhgl.app.model.Command.SelectChampionHoverCommand;
import com.zzhgl.app.model.Command.ReadyForChampionSelectCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.champions.ChampionFactory;
import com.zzhgl.app.networking.response.game.ResponseStartChampionSelection;
import com.zzhgl.app.networking.response.game.ResponseChampionSelectReady;
import com.zzhgl.app.networking.response.game.ResponseNotifyForChampionPick;
import com.zzhgl.app.networking.response.game.ResponseChampionSelectCompleted;
import com.zzhgl.app.networking.response.game.ResponseNotifyPlayerPick;
import com.zzhgl.app.networking.response.game.ResponseNotifyPlayerSelect;
import com.zzhgl.app.utility.Log;

import java.util.*;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

public class ChampionSelectState implements GameState {
    private enum Phase { WAITING_FOR_READY, SELECTING }
    
    private Phase currentPhase = Phase.WAITING_FOR_READY;
    private Set<Integer> readyPlayers = new HashSet<>();
    private List<Integer> availableChampions;
    private ScheduledExecutorService scheduler;
    private ScheduledFuture<?> timerFuture;
    
    private static final int READY_TIMEOUT_SECONDS = 60;
    private static final int SELECTION_TIMEOUT_SECONDS = 30;

    public ChampionSelectState() {
        this.availableChampions = new ArrayList<>();
        for (int i = 0; i < 12; i++) {
            availableChampions.add(i);
        }
        this.scheduler = Executors.newSingleThreadScheduledExecutor();
    }

    @Override
    public void onEnter(GameManager game) {
        Log.printf("Entering ChampionSelectState. Phase: WAITING_FOR_READY");
        
        startTimer(() -> {
            Log.printf("Ready phase timed out. Starting selection anyway.");
            startSelectionPhase(game);
        }, READY_TIMEOUT_SECONDS);
    }

    @Override
    public synchronized void handleAction(GameManager game, Command command) {
        if (currentPhase == Phase.WAITING_FOR_READY) {
            if (command instanceof ReadyForChampionSelectCommand readyCmd) {
                readyPlayers.add(readyCmd.getPlayerId());
                Log.printf("Player %d is ready for champion select. (%d/%d)", 
                           readyCmd.getPlayerId(), readyPlayers.size(), game.getPlayers().size());
                
                if (readyPlayers.size() >= game.getPlayers().size()) {
                    startSelectionPhase(game);
                }
            }
        } else if (currentPhase == Phase.SELECTING) {
            if (command instanceof SelectChampionHoverCommand hoverCmd) {
                Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
                if (hoverCmd.getPlayerId() == activePlayer.getID()) {
                    Log.printf("Player %d is hovering champion %d", hoverCmd.getPlayerId(), hoverCmd.getChampionId());
                    broadcast(game, new ResponseNotifyPlayerSelect(hoverCmd.getPlayerId(), hoverCmd.getChampionId()));
                }
            } else if (command instanceof SelectChampionCommand selectCmd) {
                Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
                if (selectCmd.getPlayerId() == activePlayer.getID()) {
                    int pickedId = selectCmd.getChampionId();
                    if (availableChampions.contains(pickedId)) {
                        processPick(game, activePlayer, pickedId);
                    } else {
                        Log.printf("Player %d tried to pick unavailable champion: %d", 
                                   activePlayer.getID(), pickedId);
                    }
                }
            }
        }
    }

    private synchronized void startSelectionPhase(GameManager game) {
        if (currentPhase != Phase.WAITING_FOR_READY) return;
        cancelTimer();

        currentPhase = Phase.SELECTING;
        game.setActivePlayerIndex(0);
        Log.printf("Assigning teams and starting champion selection.");
        
        assignTeams(game);

        // Notify all players that selection phase is ready with player info and pool
        ResponseChampionSelectReady readyResponse = new ResponseChampionSelectReady();
        readyResponse.setPlayers(game.getPlayers());
        readyResponse.setChampionPool(availableChampions);
        broadcast(game, readyResponse);

        // Notify all players that selection phase has started
        broadcast(game, new ResponseStartChampionSelection());
        
        notifyNextPlayerTurn(game);
    }

    private void notifyNextPlayerTurn(GameManager game) {
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        Log.printf("It's player %d's turn to pick.", activePlayer.getID());
        
        // Notify everyone who is picking
        broadcast(game, new ResponseNotifyForChampionPick(activePlayer.getID(), SELECTION_TIMEOUT_SECONDS));
        
        startTimer(() -> {
            Log.printf("Selection timeout for player %d. Randomly assigning.", activePlayer.getID());
            assignRandomChampion(game);
        }, SELECTION_TIMEOUT_SECONDS);
    }

    private synchronized void processPick(GameManager game, Player player, int championId) {
        if (currentPhase != Phase.SELECTING) return;
        
        // Ensure this is actually the turn of the player picking
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        if (player.getID() != activePlayer.getID()) return;

        cancelTimer();
        Champion champion = ChampionFactory.getInstance().createChampion(championId);
        player.setSelectedChampion(champion);
        availableChampions.remove(Integer.valueOf(championId));
        Log.printf("Player %d picked champion %d (%s)", 
                   player.getID(), championId, champion != null ? champion.getChampionName() : "Unknown");

        // Notify all players about the pick
        broadcast(game, new ResponseNotifyPlayerPick(player.getID(), championId));

        nextTurn(game);
    }

    private void nextTurn(GameManager game) {
        int nextIndex = game.getActivePlayerIndex() + 1;
        if (nextIndex < game.getPlayers().size()) {
            game.setActivePlayerIndex(nextIndex);
            notifyNextPlayerTurn(game);
        } else {
            Log.printf("All players have selected champions.");
            broadcast(game, new ResponseChampionSelectCompleted());
            // Transition to handshake state
            game.setState(new PreGameState());
        }
    }

    private void assignTeams(GameManager game) {
        List<Player> players = game.getPlayers();
        Collections.shuffle(players);

        // Circular alternating teams: Blue, Red, Blue, Red...
        for (int i = 0; i < players.size(); i++) {
            Player p = players.get(i);
            p.setTeam(i % 2 == 0 ? Constants.TEAM_BLUE : Constants.TEAM_RED);
            Log.printf("Player %d assigned to Team %s", 
                       p.getID(), (p.getTeam() == Constants.TEAM_BLUE ? "BLUE" : "RED"));
        }
    }

    private synchronized void assignRandomChampion(GameManager game) {
        if (currentPhase != Phase.SELECTING) return;

        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        if (activePlayer.getSelectedChampion() == null && !availableChampions.isEmpty()) {
            int randomIndex = new Random().nextInt(availableChampions.size());
            int randomChampion = availableChampions.get(randomIndex);
            processPick(game, activePlayer, randomChampion);
        }
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

    private void broadcast(GameManager game, com.zzhgl.app.networking.response.GameResponse response) {
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }
    }

    @Override
    public void onExit(GameManager game) {
        Log.printf("Exiting ChampionSelectState.");
        cancelTimer();
        scheduler.shutdown();
    }

    @Override
    public void onPause(GameManager game) {
        Log.printf("Pausing ChampionSelectState.");
    }

    @Override
    public void onResume(GameManager game) {
        Log.printf("Resuming ChampionSelectState.");
    }
}
