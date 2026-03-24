package com.zzhgl.app.model.effects;

import com.zzhgl.app.model.actions.DealDamageAction;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

import java.util.List;

/**
 * Effect for Clara's "Svarog's Revenge" skill.
 * If judgement is NOT a Heart, deals 1 damage to the attacker.
 */
public class SvarogsRevengeEffect extends AbstractEffect {

    private final Player attacker;

    public SvarogsRevengeEffect(Player caster, Player attacker) {
        super(null, caster, caster);
        this.attacker = attacker;
    }

    @Override
    public String getName() {
        return "Svarog's Revenge (Judgement)";
    }

    @Override
    public boolean evaluateJudgement(GameManager game, AbstractCard judgementCard) {
        // If it is non a heart
        return judgementCard.getSuit() != AbstractCard.Suit.HEART;
    }

    @Override
    public List<GameAction> applyConsequence(GameManager game, AbstractCard judgementCard) {
        Log.printf("SvarogsRevengeEffect successful! Player %d counter-attacks Player %d", caster.getID(), attacker.getID());
        
        int damage = 1; // Default to 1 damage
        if (caster.getSelectedChampion() != null) {
            damage = caster.getSelectedChampion().getAttack();
        }

        return List.of(
            new DealDamageAction(caster, attacker, damage)
        );
    }
}
