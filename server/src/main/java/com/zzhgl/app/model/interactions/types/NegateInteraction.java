package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.utility.Log;

public class NegateInteraction extends AbstractInteraction {

    private AbstractInteraction targetInteraction;

    public NegateInteraction(Player caster, AbstractCard card, AbstractInteraction targetInteraction) {
        super(caster, targetInteraction.getCaster(), card, true); // Negate is itself negatable
        this.targetInteraction = targetInteraction;
    }

    public AbstractInteraction getTargetInteraction() {
        return targetInteraction;
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating NegateInteraction from %s. Canceling interaction.", caster.getUsername());
        if (targetInteraction != null) {
            targetInteraction.cancel();
        }
    }
}
