package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;

/**
 * Skill 2: On Damage Taken, draw two cards. Optional.
 */
public class DrawOnDamageSkill extends AbstractSkill {
    
    public DrawOnDamageSkill() {
        super(2, "Vengeful Draw", true);
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
    public boolean execute(GameManager game, Player owner, GameEvent event, Object data) {
        // 'data' would contain the chosen target player ID from user input
        if (data instanceof Integer targetPlayerId) {
            Player target = game.getPlayerMap().get(targetPlayerId);
            if (target != null) {
                game.drawCards(target, 2);
                return true;
            }
        }
        return false;
    }
}
