package com.zzhgl.app.model.core;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.networking.response.game.ResponseGameStart;
import com.zzhgl.app.networking.response.game.ResponseMatchState;
import com.zzhgl.app.model.States.ChampionSelectState;

public class Room {
    private String id;
    private String name;
    private int hostPlayerId;
    private List<Player> players;
    private boolean inGame = false;
    private GameManager gameManager;
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();

    public Room(String id, String name, int hostPlayerId) {
        this.id = id;
        this.name = name;
        this.hostPlayerId = hostPlayerId;
        this.players = new ArrayList<>();
    }

    public synchronized void startGame() {
        if (!inGame) {
            inGame = true;
            broadcast(new ResponseGameStart());
            this.gameManager = new GameManager(players);
            this.gameManager.setState(new ChampionSelectState());
        }
    }

    public GameManager getGameManager() { return gameManager; }

    public String getId() { return id; }
    public String getName() { return name; }
    public int getHostPlayerId() { return hostPlayerId; }
    public void setHostPlayerId(int hostPlayerId) { this.hostPlayerId = hostPlayerId; }
    public boolean isInGame() { return inGame; }
    public void setInGame(boolean inGame) { this.inGame = inGame; }

    public synchronized void addPlayer(Player player) {
        if (!players.contains(player)) {
            players.add(player);
        }
    }

    public synchronized void removePlayer(Player player) {
        players.remove(player);
    }

    public synchronized List<Player> getPlayers() {
        return new ArrayList<>(players);
    }

    public synchronized boolean isEmpty() {
        return players.isEmpty();
    }
    
    public synchronized void broadcast(GameResponse response) {
        for (Player p : players) {
            p.addResponseForUpdate(response);
        }
    }
    
    public synchronized void broadcast(GameResponse response, int excludePlayerId) {
        for (Player p : players) {
            if (p.getID() != excludePlayerId) {
                p.addResponseForUpdate(response);
            }
        }
    }

    public synchronized void broadcastMatchState() {
        if (!inGame) return;
        int connectedCount = 0;
        for (Player p : players) {
            if (p.isConnected()) {
                connectedCount++;
            }
        }
        ResponseMatchState stateResponse = new ResponseMatchState(connectedCount, players.size());
        broadcast(stateResponse);
    }

    public void stop() {
        scheduler.shutdown();
    }
}
