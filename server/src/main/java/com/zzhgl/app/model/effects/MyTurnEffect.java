package com.zzhgl.app.model.effects;

import com.zzhgl.app.model.States.JudgementState;
import com.zzhgl.app.model.actions.AddCardToHandAction;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.PushStateAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

import java.util.List;

/**
 * Effect for Seele's "My Turn" skill.
 * If judgement is Spade/Club, adds card to hand and triggers another judgement.
 */
public class MyTurnEffect extends AbstractEffect {

    public MyTurnEffect(Player caster) {
        super(null, caster, caster);
    }

    @Override
    public String getName() {
        return "My Turn (Judgement)";
    }

    @Override
    public boolean evaluateJudgement(GameManager game, AbstractCard judgementCard) {
        return judgementCard.getSuit() == AbstractCard.Suit.SPADE || judgementCard.getSuit() == AbstractCard.Suit.CLUB;
    }

    @Override
    public List<GameAction> applyConsequence(GameManager game, AbstractCard judgementCard) {
        Log.printf("MyTurnEffect successful! Player %d takes card %s", caster.getID(), judgementCard);
        
        return List.of(
            new AddCardToHandAction(caster, judgementCard),
            new PushStateAction(new JudgementState(new MyTurnEffect(caster))) // Trigger next judgement
        );
    }
}
