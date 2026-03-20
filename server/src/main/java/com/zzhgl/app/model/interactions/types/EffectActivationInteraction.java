package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.effects.AbstractEffect;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.States.JudgementState;
import com.zzhgl.app.utility.Log;

public class EffectActivationInteraction extends AbstractInteraction {
    private final AbstractEffect effect;

    public EffectActivationInteraction(AbstractEffect effect) {
        // Source is technically the effect or the environment, but target is the one suffering it.
        super(effect.getCaster(), effect.getTarget(), effect.getSourceCard(), true);
        this.effect = effect;
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("EffectActivationInteraction resolving. Transitioning to JudgementState for %s", effect.getName());
        // If we reach here, it was NOT negated. Proceed to Judgement for this effect.
        game.pushState(new JudgementState(effect));
    }
}
