package com.zzhgl.app.model.core;

import java.util.LinkedList;
import java.util.Queue;
import java.util.List;
import java.util.ArrayList;

import com.zzhgl.app.core.GameClient;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.effects.AbstractEffect;

/**
 * The Player class holds important information about the player including, most
 * importantly, the account. Such information includes the username, password,
 * email, and the player ID.
 */
public class Player {

    private int player_id;
    private String username;
    private String sessionToken;
    private GameClient client; // References GameClient instance

    private long disconnectedTime; // Time when client disconnected
    private boolean isConnected; // Connection status
    private Queue<GameResponse> updates; // Pending responses for this player

    private Room currentRoom;
    private Champion selectedChampion;
    private Hand hand;
    private int team = -1; // -1 for none, 0 for Blue, 1 for Red
    private List<AbstractEffect> activeEffects = new ArrayList<>();

    public List<AbstractEffect> getActiveEffects() {
        return activeEffects;
    }

    public void addEffect(AbstractEffect effect) {
        activeEffects.add(effect);
    }

    public void removeEffect(AbstractEffect effect) {
        activeEffects.remove(effect);
    }

    public int getTeam() {
        return team;
    }

    public void setTeam(int team) {
        this.team = team;
    }

    public Champion getSelectedChampion() {
        return selectedChampion;
    }

    public void setSelectedChampion(Champion selectedChampion) {
        this.selectedChampion = selectedChampion;
    }

    public Hand getHand() {
        return hand;
    }

    public Room getCurrentRoom() {
        return currentRoom;
    }

    public void setCurrentRoom(Room currentRoom) {
        this.currentRoom = currentRoom;
    }

    // Add these methods:
    public void setDisconnectedTime(long time) {
        this.disconnectedTime = time;
    }

    public long getDisconnectedTime() {
        return disconnectedTime;
    }

    public boolean isConnected() {
        return isConnected;
    }

    public void setConnected(boolean connected) {
        isConnected = connected;
    }

    public Player(int player_id) {
        this.player_id = player_id;
        this.updates = new LinkedList<GameResponse>();
        this.hand = new Hand();
    }

    public Player(int player_id, String username) {
        this.player_id = player_id;
        this.username = username;
        this.isConnected = true; // Initially connected
        this.updates = new LinkedList<GameResponse>();
        this.hand = new Hand();
    }

    public int getID() {
        return player_id;
    }

    public int setID(int player_id) {
        return this.player_id = player_id;
    }

    public String getUsername() {
        return username;
    }

    public void setSessionToken(String token) {
        this.sessionToken = token;
    }

    public String getSessionToken() {
        return sessionToken;
    }

    public String setUsername(String username) {
        return this.username = username;
    }

    public GameClient getClient() {
        return client;
    }

    public GameClient setClient(GameClient client) {
        synchronized (this) {
            this.client = client;
            if (this.client != null) {
                this.isConnected = true; // Mark as connected when client is set
                // Flush pending updates when a new client connects
                while (!updates.isEmpty()) {
                    try {
                        this.client.send(updates.peek());
                        updates.poll();
                    } catch (Exception e) {
                        break; // Stop flushing if socket fails
                    }
                }
            }
        }
        return this.client;
    }
    
    public boolean addResponseForUpdate(GameResponse response) {
        synchronized (this) {
            if (isConnected && client != null) {
                try {
                    client.send(response);
                    return true;
                } catch (Exception e) {
                    // Socket error: queue it for later
                    return updates.add(response);
                }
            }
            // Client not connected: queue it
            return updates.add(response);
        }
    }

    public Queue<GameResponse> getUpdates() {
        Queue<GameResponse> responseList = null;
        synchronized (this) {
            responseList = updates;
            updates = new LinkedList<GameResponse>();
        }
        return responseList;
    }

    @Override
    public String toString() {
        return "Player{" +
                "player_id=" + player_id +
                ", username='" + username + '\'' +
                '}';
    }

    @Override
    public boolean equals(Object obj) {
        if (this == obj) {
            return true;
        }
        if (!(obj instanceof Player)) {
            return false;
        }
        Player other = (Player) obj;
        return this.player_id == other.player_id;
    }   
}