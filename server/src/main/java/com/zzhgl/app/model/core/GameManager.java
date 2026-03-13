package com.zzhgl.app.model.core;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.States.GameState;
import com.zzhgl.app.model.cards.AbstractCard;
import java.util.ArrayDeque;
import java.util.Deque;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * GameManager handles the game logic and state.
 */
public class GameManager {
    private List<Player> players;
    private Map<Integer, Player> playerMap;
    private int activePlayerIndex;
    private int turnCounter;
    private boolean inGame;
    private Deque<GameState> stateStack;
    private Deck deck;
    private Pile drawPile;
    private Pile discardPile;

    public GameManager(List<Player> players) {
        this.players = players;
        this.playerMap = new HashMap<>();
        for (Player player : players) {
            this.playerMap.put(player.getID(), player);
        }
        this.activePlayerIndex = 0;
        this.turnCounter = 1;
        this.inGame = true;
        this.stateStack = new ArrayDeque<>();
        this.deck = new Deck();
        this.drawPile = new Pile();
        this.discardPile = new Pile();
        
        // Initialize draw pile with cards from deck
        this.drawPile.addCards(deck.getCards());
        this.drawPile.shuffle();
    }

    public Pile getDrawPile() {
        return drawPile;
    }

    public Pile getDiscardPile() {
        return discardPile;
    }

    public void pushState(GameState newState) {
        if (!stateStack.isEmpty()) {
            stateStack.peek().onPause(this);
        }
        stateStack.push(newState);
        newState.onEnter(this);
    }

    public void popState() {
        if (!stateStack.isEmpty()) {
            stateStack.pop().onExit(this);
        }
        if (!stateStack.isEmpty()) {
            stateStack.peek().onResume(this);
        }
    }

    public void setState(GameState newState) {
        while (!stateStack.isEmpty()) {
            stateStack.pop().onExit(this);
        }
        pushState(newState);
    }

    public void handleAction(Command command) {
        GameState currentState = getCurrentState();
        if (currentState != null) {
            currentState.handleAction(this, command);
        }
    }

    public GameState getCurrentState() {
        return stateStack.peek();
    }

    public List<Player> getPlayers() {
        return players;
    }

    public Map<Integer, Player> getPlayerMap() {
        return playerMap;
    }

    public int getActivePlayerIndex() {
        return activePlayerIndex;
    }

    public void setActivePlayerIndex(int activePlayerIndex) {
        this.activePlayerIndex = activePlayerIndex;
    }

    public int getTurnCounter() {
        return turnCounter;
    }

    public void setTurnCounter(int turnCounter) {
        this.turnCounter = turnCounter;
    }

    public boolean isInGame() {
        return inGame;
    }

    public void setInGame(boolean inGame) {
        this.inGame = inGame;
    }
}
