package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.SetExtraDrawAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import java.util.List;

/**
 * Skill 1: Military Might. Increase draw count by 1 during start turn.
 */
public class DrawOneOnTurnBeginSkill extends AbstractSkill {
    
    public DrawOneOnTurnBeginSkill() {
        super(1, "Military Might", false);
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
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        return List.of(new SetExtraDrawAction(1));
    }
}
