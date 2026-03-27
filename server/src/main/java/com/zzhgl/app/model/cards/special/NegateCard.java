package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.utility.Log;
import java.util.List;

public class NegateCard extends AbstractSpecialCard {
    public NegateCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.NEGATE);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        // Can only be played if the top interaction is negatable
        AbstractInteraction top = game.getInteractionStack().peek();
        return top != null && top.isNegatable();
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        // // Negate needs to target the interaction currently on top of the stack.
        // // During the NegationState, the top interaction is peeked and passed or 
        // // we assume we are negating whatever is currently on top.
        
        // AbstractInteraction topInteraction = game.getInteractionStack().peek();
        // if (topInteraction != null && topInteraction.isNegatable()) {
        //     game.getInteractionStack().push(new NegateInteraction(caster, this, topInteraction));
        // } else {
        //     // Ideally, this validation happens before allowing the play, 
        //     // but this is a safeguard.
        //     Log.printf_e("Player %d tried to play NegateCard, but there is no negatable interaction on the stack.", caster.getID());
        // }
    }
}
