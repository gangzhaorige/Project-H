package com.zzhgl.app.model.core;

import com.zzhgl.app.core.NetworkManager;
import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.States.ActionResolveState;
import com.zzhgl.app.model.States.GameState;
import com.zzhgl.app.model.States.InteractionResolutionState;
import com.zzhgl.app.model.States.SkillResolutionState;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.skills.AbstractSkill;
import com.zzhgl.app.networking.response.game.ResponseDrawCard;
import com.zzhgl.app.networking.response.game.ResponseDrawCardOther;
import com.zzhgl.app.networking.response.game.ResponseGameState;

import com.zzhgl.app.model.actions.GameAction;
import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Deque;
import java.util.EnumMap;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Queue;

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
    private Queue<GameAction> actionQueue = new LinkedList<>();
    private Deck deck;
    private Pile drawPile;
    private Pile discardPile;
    private InteractionStack interactionStack;
    private boolean skipActionPhase = false;
    private boolean skipDrawPhase = false;
    private int extraDrawCount = 0;

    // Subscription-based skill registry
    private final Map<GameEvent.EventType, List<SkillSubscription>> skillRegistry = new EnumMap<>(GameEvent.EventType.class);

    public record SkillSubscription(Player owner, AbstractSkill skill) {}

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
        this.interactionStack = new InteractionStack();
        
        // Initialize draw pile with cards from deck
        this.drawPile.addCards(deck.getCards());
        this.drawPile.shuffle();
    }

    public void registerSkill(Player owner, AbstractSkill skill) {
        GameEvent.EventType type = skill.getSubscribedEvent();
        skillRegistry.computeIfAbsent(type, k -> new ArrayList<>())
                     .add(new SkillSubscription(owner, skill));
    }

    public void unregisterSkill(Player owner, AbstractSkill skill) {
        GameEvent.EventType type = skill.getSubscribedEvent();
        List<SkillSubscription> subs = skillRegistry.get(type);
        if (subs != null) {
            subs.removeIf(s -> s.owner().equals(owner) && s.skill().equals(skill));
        }
    }

    public InteractionStack getInteractionStack() {
        return interactionStack;
    }

    public Queue<GameAction> getActionQueue() {
        return actionQueue;
    }

    public boolean hasBlockingAction() {
        return actionQueue.stream().anyMatch(GameAction::isBlocking);
    }

    public boolean isSkipActionPhase() { return skipActionPhase; }
    public void setSkipActionPhase(boolean skip) { this.skipActionPhase = skip; }

    public boolean isSkipDrawPhase() { return skipDrawPhase; }
    public void setSkipDrawPhase(boolean skip) { this.skipDrawPhase = skip; }

    public int getExtraDrawCount() { return extraDrawCount; }
    public void setExtraDrawCount(int count) { this.extraDrawCount = count; }

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
        List<SkillSubscription> subscribers = skillRegistry.get(event.getType());
        if (subscribers == null || subscribers.isEmpty()) return;

        SkillResolutionState resolutionState = new SkillResolutionState();
        boolean hasSkills = false;
        
        // Loop through players starting from the active player to maintain priority order
        int totalPlayers = players.size();
        for (int i = 0; i < totalPlayers; i++) {
            int index = (activePlayerIndex + i) % totalPlayers;
            Player p = players.get(index);
            
            // Check if this player is in the subscriber list for this event
            for (SkillSubscription sub : subscribers) {
                if (sub.owner().equals(p)) {
                    // Subscription exists; now check if specific triggering conditions are met
                    if (sub.skill().canTrigger(this, event, p)) {
                        resolutionState.addSkill(p, sub.skill(), event);
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
     * Resolves the current interaction stack.
     */
    public void resolveStack() {
        if (interactionStack.isEmpty()) return;
        
        // If we're already in InteractionResolutionState, let it handle the new interactions
        if (getCurrentState() instanceof InteractionResolutionState) return;

        // Instead of passing a list, the state now pulls from the GameManager's stack
        pushState(new InteractionResolutionState());
    }

    /**
     * Pushes ActionResolveState to process pending actions if not already processing.
     */
    public void resolveActions() {
        if (actionQueue.isEmpty()) return;
        if (getCurrentState() instanceof ActionResolveState) return;
        pushState(new ActionResolveState());
    }

    public int getDistance(int p1Id, int p2Id) {
        int idx1 = -1;
        int idx2 = -1;

        for (int i = 0; i < players.size(); i++) {
            if (players.get(i).getID() == p1Id) idx1 = i;
            if (players.get(i).getID() == p2Id) idx2 = i;
        }

        if (idx1 == -1 || idx2 == -1) return 999;

        int n = players.size();
        int diff = Math.abs(idx1 - idx2);
        
        // Shortest distance in a circle
        return Math.min(diff, n - diff);
    }

    public List<Player> getAlivePlayers() {
        List<Player> alive = new ArrayList<>();
        for (Player p : players) {
            if (p.getSelectedChampion() != null && p.getSelectedChampion().getCurHP() > 0) {
                alive.add(p);
            }
        }
        return alive;
    }

    public List<Player> getAlivePlayersClockwise(Player startPlayer) {
        List<Player> alive = new ArrayList<>();
        int startIndex = players.indexOf(startPlayer);
        if (startIndex == -1) return alive;
        
        int n = players.size();
        for (int i = 1; i <= n; i++) {
            int index = (startIndex + i) % n;
            Player p = players.get(index);
            if (p.getSelectedChampion() != null && p.getSelectedChampion().getCurHP() > 0) {
                alive.add(p);
            }
        }
        return alive;
    }

    public void pushState(GameState newState) {
        if (!stateStack.isEmpty()) {
            stateStack.peek().onPause(this);
        }
        stateStack.push(newState);
        
        // Broadcast state change
        broadcastGameState(newState.getClass().getSimpleName());
        
        newState.onEnter(this);
    }

    public void popState() {
        if (!stateStack.isEmpty()) {
            stateStack.pop().onExit(this);
        }
        if (!stateStack.isEmpty()) {
            GameState top = stateStack.peek();
            
            // Broadcast state change back to the previous state
            broadcastGameState(top.getClass().getSimpleName());
            
            top.onResume(this);
        }
    }

    public void setState(GameState newState) {
        while (!stateStack.isEmpty()) {
            stateStack.pop().onExit(this);
        }
        pushState(newState);
    }

    private void broadcastGameState(String stateName) {
        com.zzhgl.app.networking.response.game.ResponseGameState response = 
            new com.zzhgl.app.networking.response.game.ResponseGameState(stateName);
        for (Player p : players) {
            p.addResponseForUpdate(response);
        }
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

    /**
     * Finds the first state of a specific type in the stack, starting from the top.
     */
    @SuppressWarnings("unchecked")
    public <T extends GameState> T findState(Class<T> stateClass) {
        for (GameState state : stateStack) {
            if (stateClass.isInstance(state)) {
                return (T) state;
            }
        }
        return null;
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
