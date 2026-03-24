package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.States.JudgementState;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.PushStateAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.effects.SvarogsRevengeEffect;
import com.zzhgl.app.utility.Log;

import java.util.List;

/**
 * Skill: Svarog's Revenge (Clara). Optional on DAMAGE_TAKEN.
 * If accepted:
 * - Enter JudgementState.
 * - If non-Heart, deal damage to the attacker.
 */
public class SvarogsRevengeSkill extends AbstractSkill {

    public SvarogsRevengeSkill() {
        super(7, "Svarog's Revenge", true); // ID 7, Optional
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
        Player attacker = (Player) event.getParam("source");
        
        Log.printf("Clara (Player %d) uses skill: %s against Player %d!", owner.getID(), getName(), attacker != null ? attacker.getID() : -1);
        
        if (attacker == null) {
            return List.of(); // Cannot revenge if there is no attacker
        }

        return List.of(
            new PushStateAction(new JudgementState(new SvarogsRevengeEffect(owner, attacker)))
        );
    }
}
