package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.DrawCardAction;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import java.util.List;

/**
 * Skill 2: On Damage Taken, draw two cards. Optional.
 */
public class DrawOnDamageSkill extends AbstractSkill {
    
    public DrawOnDamageSkill() {
        super(2, "Vengeful Draw", true);
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.DAMAGE_TAKEN;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        if (event.getType() == GameEvent.EventType.DAMAGE_TAKEN) {
            Player target = (Player) event.getParam("target");
            return target != null && target.getID() == owner.getID();
        }
        return false;
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        return List.of(new DrawCardAction(owner, 2));
    }
}
