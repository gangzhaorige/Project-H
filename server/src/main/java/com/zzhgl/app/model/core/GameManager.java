package com.zzhgl.app.model.core;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.States.GameState;
import com.zzhgl.app.model.States.InteractionResolutionState;
import com.zzhgl.app.model.States.SkillResolutionState;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.skills.AbstractSkill;
import com.zzhgl.app.networking.response.game.ResponseDrawCard;
import com.zzhgl.app.networking.response.game.ResponseDrawCardOther;
import java.util.ArrayDeque;
import java.util.ArrayList;
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
        this.activePlayerIndex = -1;
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

    /**
     * Draws a specified number of cards for a player and notifies everyone.
     */
    public void drawCards(Player player, int count) {
        List<AbstractCard> drawn = new ArrayList<>();
        for (int i = 0; i < count; i++) {
            AbstractCard card = drawPile.draw();
            if (card != null) {
                player.getHand().addCard(card);
                drawn.add(card);
            }
        }

        if (drawn.isEmpty()) return;

        // 1. Send the actual card data to the player
        player.addResponseForUpdate(new ResponseDrawCard(drawn));

        // 2. Notify all other players about the draw count
        ResponseDrawCardOther otherRes = new ResponseDrawCardOther(player.getID(), drawn.size());
        for (Player p : players) {
            if (p.getID() != player.getID()) {
                p.addResponseForUpdate(otherRes);
            }
        }
    }

    /**
     * Emits a game event and allows all champions to react sequentially.
     */
    public void emitEvent(GameEvent event) {
        SkillResolutionState resolutionState = new SkillResolutionState();
        int totalPlayers = players.size();
        boolean hasSkills = false;
        
        // Loop through players starting from the active player
        for (int i = 0; i < totalPlayers; i++) {
            int index = (activePlayerIndex + i) % totalPlayers;
            Player p = players.get(index);
            
            if (p.getSelectedChampion() != null) {
                for (AbstractSkill skill : p.getSelectedChampion().getSkills()) {
                    if (skill.canTrigger(this, event, p)) {
                        resolutionState.addSkill(p, skill, event);
                        hasSkills = true;
                    }
                }
            }
        }

        // Only push if there are actually skills to resolve
        if (hasSkills) {
            pushState(resolutionState);
        }
    }

    /**
     * Resolves a list of interactions sequentially.
     */
    public void resolveInteractions(List<AbstractInteraction> interactions) {
        if (interactions == null || interactions.isEmpty()) return;
        pushState(new InteractionResolutionState(interactions));
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
