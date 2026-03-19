package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.utility.Log;
import com.zzhgl.app.model.champions.Champion;

public class HealInteraction extends AbstractInteraction {

    private int healAmount;

    public HealInteraction(Player caster, Player target, AbstractCard card, int healAmount, boolean negatable) {
        super(caster, target, card, negatable);
        this.healAmount = healAmount;
    }

    @Override
    public void evaluate(GameManager game) {
        Champion champ = target.getSelectedChampion();
        if (champ != null) {
            int newHp = Math.min(champ.getMaxHP(), champ.getCurHP() + healAmount);
            champ.setCurHP(newHp);
            Log.printf("Evaluating HealInteraction: %s healed for %d. New HP: %d", target.getUsername(), healAmount, newHp);
            // Consider emitting a HEALED event here
        }
    }
}
