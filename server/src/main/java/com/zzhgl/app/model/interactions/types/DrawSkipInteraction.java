package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.effects.DrawSkipEffect;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.utility.Log;

public class DrawSkipInteraction extends AbstractInteraction {

    public DrawSkipInteraction(Player source, Player target, AbstractCard sourceCard) {
        super(source, target, sourceCard, true); // It is negatable
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("DrawSkipInteraction resolving. Player %d attaches DrawSkip to Player %d", caster.getID(), target.getID());
        target.addEffect(new DrawSkipEffect(card, caster, target));
    }
}
