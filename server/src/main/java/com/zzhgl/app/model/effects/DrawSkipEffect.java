package com.zzhgl.app.model.effects;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.SetFlagAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;
import java.util.List;

/**
 * Effect that skips the draw phase if judgement fails.
 */
public class DrawSkipEffect extends AbstractEffect {

    public DrawSkipEffect(AbstractCard sourceCard, Player caster, Player target) {
        super(sourceCard, caster, target);
    }

    @Override
    public String getName() {
        return "DrawSkip";
    }

    @Override
    public boolean evaluateJudgement(GameManager game, AbstractCard judgementCard) {
        Log.printf("Judging DrawSkip for %d. Card is %s", target.getID(), judgementCard.getSuit());
        // Skip if NOT Hearts
        return judgementCard.getSuit() != AbstractCard.Suit.DIAMOND;
    }

    @Override
    public List<GameAction> applyConsequence(GameManager game, AbstractCard judgementCard) {
        Log.printf("DrawSkip effect activated! Player %d loses their draw phase.", target.getID());
        return List.of(new SetFlagAction("skipDrawPhase", GameManager::setSkipDrawPhase, true));
    }
}
