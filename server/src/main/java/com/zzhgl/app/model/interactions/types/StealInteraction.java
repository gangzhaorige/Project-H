package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.States.StealState;
import com.zzhgl.app.utility.Log;

public class StealInteraction extends AbstractInteraction {

    public StealInteraction(Player caster, Player target, AbstractCard card) {
        super(caster, target, card, true);
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating StealInteraction from %s to %s.", caster.getUsername(), target.getUsername());
        
        if (target.getHand().getCards().isEmpty()) {
            Log.printf("Target has no cards to steal.");
            return;
        }

        // Push StealState to handle the selection
        game.pushState(new StealState(caster, target));
    }
}
