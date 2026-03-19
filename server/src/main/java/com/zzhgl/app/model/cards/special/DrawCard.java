package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.Draw2Interaction;
import java.util.List;

/**
 * DrawCard represents a special card that lets the player draw 2 cards.
 */
public class DrawCard extends AbstractSpecialCard {
    public DrawCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.DRAW);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        return targetIds == null || targetIds.isEmpty();
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        game.getInteractionStack().push(new Draw2Interaction(caster, this));
    }
}

