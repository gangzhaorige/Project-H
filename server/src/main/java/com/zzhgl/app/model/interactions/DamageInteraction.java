package com.zzhgl.app.model.interactions;

import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;

/**
 * DamageInteraction handles dealing damage to a player's champion.
 */
public class DamageInteraction extends AbstractInteraction {
    private int amount;

    public DamageInteraction(Player source, Player target, int amount) {
        super(source, target);
        this.amount = amount;
    }

    @Override
    public void execute(GameManager game) {
        Champion targetChamp = target.getSelectedChampion();
        if (targetChamp != null) {
            int newHp = Math.max(0, targetChamp.getCurHP() - amount);
            targetChamp.setCurHP(newHp);
            
            // Emit the event so skills can react
            game.emitEvent(new GameEvent(GameEvent.EventType.DAMAGE_TAKEN)
                .setParam("target", target)
                .setParam("source", source)
                .setParam("amount", amount));
        }
    }
}
