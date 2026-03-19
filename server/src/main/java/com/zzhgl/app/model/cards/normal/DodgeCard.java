package com.zzhgl.app.model.cards.normal;

import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import java.util.List;

/**
 * DodgeCard represents a normal card used for dodging attacks.
 */
public class DodgeCard extends AbstractNormalCard {
    public DodgeCard(int id, Suit suit, int value) {
        super(id, suit, value, NormalType.DODGE);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        return false; // Typically played as response
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        // Dodge cards are typically played in response to an Attack, not during the main phase.
        // The interaction logic will handle Dodge directly from the hand during a prompt.
        // For now, if played directly, it might just do nothing or be invalid.
    }
}
