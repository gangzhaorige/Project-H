package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import java.util.List;

/**
 * AbstractSkill represents a unique skill that can be possessed by a champion.
 * Skills are now fully reactive to GameEvents.
 */
public abstract class AbstractSkill {
    protected int id;
    protected String name;
    protected boolean isOptional;

    public AbstractSkill(int id, String name, boolean isOptional) {
        this.id = id;
        this.name = name;
        this.isOptional = isOptional;
    }

    public int getId() { return id; }
    public String getName() { return name; }
    public boolean isOptional() { return isOptional; }

    /**
     * Defines the event this skill is interested in.
     */
    public abstract GameEvent.EventType getSubscribedEvent();

    /**
     * Checks if this this skill wants to react to the given event.
     */
    public abstract boolean canTrigger(GameManager game, GameEvent event, Player owner);

    /**
     * Returns a list of actions this skill performs.
     * @param data Optional data from user response (e.g., target ID).
     * @return List of GameActions to be executed by the state machine.
     */
    public abstract List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data);

    /**
     * Validates if the skill can be manually activated.
     * @param discardCardIds List of card IDs to discard for cost.
     * @param targetIds List of player IDs targeted by the skill.
     * @return true if valid, false otherwise.
     */
    public boolean validateActivation(GameManager game, Player owner, List<Integer> discardCardIds, List<Integer> targetIds) {
        return false; // Default: cannot be manually activated
    }

    /**
     * Executes the skill manually (Active skills) by pushing an interaction to the stack.
     * @param discardCardIds List of card IDs to discard for cost.
     * @param targetIds List of player IDs targeted by the skill.
     */
    public void activate(GameManager game, Player owner, List<Integer> discardCardIds, List<Integer> targetIds) {
        // Default implementation does nothing
    }

    /**
     * Called when the response timer for this skill expires.
     */
    public void onTimeout(GameManager game, Player owner, GameEvent event) {
        // Default: do nothing
    }
}

