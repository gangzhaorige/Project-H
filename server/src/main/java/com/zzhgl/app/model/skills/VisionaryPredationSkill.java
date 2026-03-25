package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.DrawCardAction;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

import java.util.List;

/**
 * Skill: Ready to Shine (Bronya). Passive.
 * At the end of the turn, draw 1 card.
 */
public class VisionaryPredationSkill extends AbstractSkill {

    public VisionaryPredationSkill() {
        super(9, "Visionary Predation", false); // ID 9, Not optional
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.TURN_END;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        if (event.getType() != GameEvent.EventType.TURN_END) return false;

        // Trigger only if it's the owner's turn that just ended
        Player activePlayer = game.getPlayers().get(game.getActivePlayerIndex());
        return owner.equals(activePlayer);
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        Log.printf("(Player %d) uses passive: %s! Drawing 1 card at turn end.", owner.getID(), getName());
        return List.of(new DrawCardAction(owner, 1));
    }
}
