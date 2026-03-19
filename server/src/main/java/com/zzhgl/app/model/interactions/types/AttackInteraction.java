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

    public AttackInteraction(Player caster, Player target, AbstractCard card, int damage) {
        super(caster, target, card, false); // Attack is not negatable
        this.damage = damage;
    }

    public int getDamage() {
        return damage;
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating AttackInteraction: %s attacks %s for %d damage.", caster.getUsername(), target.getUsername(), damage);
        
        // Push a DefendPromptState asking for a Dodge card.
        game.pushState(new DefendPromptState(target, caster, this, damage, AbstractNormalCard.NormalType.DODGE));
    }
}
