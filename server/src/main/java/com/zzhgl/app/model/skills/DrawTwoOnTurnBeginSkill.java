package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;

/**
 * Skill 1: Draw 2 cards during start turn (Automatic).
 */
public class DrawTwoOnTurnBeginSkill extends AbstractSkill {
    
    public DrawTwoOnTurnBeginSkill() {
        super(1, "Beginning Wisdom", false);
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.TURN_BEGIN;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        if (event.getType() == GameEvent.EventType.TURN_BEGIN) {
            int activeIndex = game.getActivePlayerIndex();
            Player active = game.getPlayers().get(activeIndex);
            return active != null && active.getID() == owner.getID();
        }
        return false;
    }

    @Override
    public boolean execute(GameManager game, Player owner, GameEvent event, Object data) {
        game.drawCards(owner, 2);
        return true;
    }
}
