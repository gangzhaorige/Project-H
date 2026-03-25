package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.States.DismantleState;
import com.zzhgl.app.utility.Log;

public class DismantleInteraction extends AbstractInteraction {

    public DismantleInteraction(Player caster, Player target, AbstractCard card) {
        super(caster, target, card, true);
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating DismantleInteraction from %s to %s.", caster.getUsername(), target.getUsername());
        
        if (target.getHand().getCards().isEmpty()) {
            Log.printf("Target has no cards to dismantle.");
            return;
        }

        // Push DismantleState to handle the selection
        game.pushState(new DismantleState(caster, target));
    }
}
