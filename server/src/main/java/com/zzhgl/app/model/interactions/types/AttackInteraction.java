package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.States.DefendPromptState;
import com.zzhgl.app.utility.Log;

public class AttackInteraction extends AbstractInteraction {

    private int damage;
    private int requiredDefenseAmount;

    public AttackInteraction(Player caster, Player target, AbstractCard card, int damage, int requiredDefenseAmount) {
        super(caster, target, card, false); // Attack is not negatable
        this.damage = damage;
        this.requiredDefenseAmount = requiredDefenseAmount;
    }

    public int getDamage() {
        return damage;
    }

    public int getRequiredDefenseAmount() {
        return requiredDefenseAmount;
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating AttackInteraction: %s attacks %s for %d damage. Required defense: %d", 
                   caster.getUsername(), target.getUsername(), damage, requiredDefenseAmount);
        
        // Push a DefendPromptState asking for Dodge cards.
        game.pushState(new DefendPromptState(target, caster, this, damage, AbstractNormalCard.NormalType.DODGE, requiredDefenseAmount));
    }
}
