package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.States.DefendPromptState;
import com.zzhgl.app.utility.Log;

public class ArrowInteraction extends AbstractInteraction {

    public ArrowInteraction(Player caster, Player target, AbstractCard card) {
        super(caster, target, card, true); // Arrow is negatable
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating ArrowInteraction from %s targeting %s.", caster.getUsername(), target.getUsername());
        
        // Push DefendPromptState asking for a Dodge card. Default damage is 1.
        game.pushState(new DefendPromptState(target, caster, this, 1, AbstractNormalCard.NormalType.DODGE, 1));
    }
}
