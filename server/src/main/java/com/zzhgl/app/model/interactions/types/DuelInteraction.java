package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.States.DuelPromptState;
import com.zzhgl.app.utility.Log;

public class DuelInteraction extends AbstractInteraction {

    private int damage;

    public DuelInteraction(Player caster, Player target, AbstractCard card, int damage) {
        super(caster, target, card, true); // Duel is negatable
        this.damage = damage;
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating DuelInteraction: %s challenged %s to a duel.", caster.getUsername(), target.getUsername());
        
        // Push DuelPromptState
        game.pushState(new DuelPromptState(target, caster, this, damage));
    }
}
