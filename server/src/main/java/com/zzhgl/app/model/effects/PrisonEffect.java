package com.zzhgl.app.model.effects;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.SetFlagAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;
import java.util.List;

public class PrisonEffect extends AbstractEffect {

    public PrisonEffect(AbstractCard sourceCard, Player caster, Player target) {
        super(sourceCard, caster, target);
    }

    @Override
    public String getName() {
        return "Prison";
    }

    @Override
    public boolean evaluateJudgement(GameManager game, AbstractCard judgementCard) {
        Log.printf("Judging Prison for %d. Card is %s of %s", target.getID(), judgementCard.getClass().getSimpleName(), judgementCard.getSuit());
        // Condition: If NOT Hearts, the effect triggers (skip turn).
        return judgementCard.getSuit() != AbstractCard.Suit.HEART;
    }

    @Override
    public List<GameAction> applyConsequence(GameManager game, AbstractCard judgementCard) {
        Log.printf("Prison effect activated! Player %d loses their action phase.", target.getID());
        return List.of(new SetFlagAction("skipActionPhase", GameManager::setSkipActionPhase, true));
    }
}
