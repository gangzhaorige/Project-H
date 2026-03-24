package com.zzhgl.app.model.interactions;

import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.cards.AbstractCard;

import com.zzhgl.app.model.cards.AbstractCard;
import java.util.HashMap;
import java.util.Map;

/**
 * DamageInteraction handles dealing damage to a player's champion.
 * It is typically not negatable by itself (the Attack/Fire interaction that caused it might be).
 */
public class DamageInteraction extends AbstractInteraction {
    private int amount;
    private final Map<String, Object> extraParams = new HashMap<>();

    public DamageInteraction(Player source, Player target, AbstractCard card, int amount) {
        super(source, target, card, false); // Damage itself is not negatable, the action causing it is
        this.amount = amount;
    }

    public DamageInteraction setExtraParam(String key, Object value) {
        extraParams.put(key, value);
        return this;
    }

    @Override
    public void evaluate(GameManager game) {
        Champion targetChamp = target.getSelectedChampion();
        if (targetChamp != null) {
            int newHp = Math.max(0, targetChamp.getCurHP() - amount);
            targetChamp.setCurHP(newHp);

            // Broadcast HP update
            com.zzhgl.app.networking.response.game.ResponseChampionStatsUpdateInteger statsUpdate = new com.zzhgl.app.networking.response.game.ResponseChampionStatsUpdateInteger(
                targetChamp.getId(),
                Champion.STAT_CUR_HP,
                newHp
            );
            for (Player p : game.getPlayers()) {
                p.addResponseForUpdate(statsUpdate);
            }

            // Emit the event so skills can react
            GameEvent event = new GameEvent(GameEvent.EventType.DAMAGE_TAKEN)
                .setParam("target", target)
                .setParam("source", caster) // caster is from AbstractInteraction
                .setParam("card", card)
                .setParam("amount", amount);

            // Add any extra context parameters
            extraParams.forEach(event::setParam);

            game.emitEvent(event);
        }
    }
}

