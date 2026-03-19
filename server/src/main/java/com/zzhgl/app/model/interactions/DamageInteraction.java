package com.zzhgl.app.model.interactions;

import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.cards.AbstractCard;

/**
 * DamageInteraction handles dealing damage to a player's champion.
 * It is typically not negatable by itself (the Attack/Fire interaction that caused it might be).
 */
public class DamageInteraction extends AbstractInteraction {
    private int amount;

    public DamageInteraction(Player source, Player target, AbstractCard card, int amount) {
        super(source, target, card, false); // Damage itself is not negatable, the action causing it is
        this.amount = amount;
    }

    @Override
    public void evaluate(GameManager game) {
        Champion targetChamp = target.getSelectedChampion();
        if (targetChamp != null) {
            int newHp = Math.max(0, targetChamp.getCurHP() - amount);
            targetChamp.setCurHP(newHp);
            
            // Emit the event so skills can react
            game.emitEvent(new GameEvent(GameEvent.EventType.DAMAGE_TAKEN)
                .setParam("target", target)
                .setParam("source", caster) // caster is from AbstractInteraction
                .setParam("amount", amount));
        }
    }
}
