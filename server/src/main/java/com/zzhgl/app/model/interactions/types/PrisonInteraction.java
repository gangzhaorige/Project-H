package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.effects.PrisonEffect;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.utility.Log;

public class PrisonInteraction extends AbstractInteraction {

    public PrisonInteraction(Player source, Player target, AbstractCard sourceCard) {
        super(source, target, sourceCard, false);
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("PrisonInteraction resolving. Player %d attaches Prison to Player %d", caster.getID(), target.getID());
        target.addEffect(new PrisonEffect(card, caster, target));
    }
}
