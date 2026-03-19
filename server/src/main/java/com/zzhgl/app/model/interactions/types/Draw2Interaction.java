package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.utility.Log;

public class Draw2Interaction extends AbstractInteraction {

    public Draw2Interaction(Player caster, AbstractCard card) {
        super(caster, caster, card, true); // Draw2 is negatable
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating Draw2Interaction for %s.", caster.getUsername());
        game.drawCards(caster, 2);
    }
}
