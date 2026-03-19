package com.zzhgl.app.model.cards;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import java.util.List;

/**
 * StandardCard is a concrete implementation of AbstractCard.
 */
public class StandardCard extends AbstractCard {
    public StandardCard(int id, Suit suit, int value) {
        super(id, suit, value);
    }

    @Override
    public Category getCategory() {
        return Category.STANDARD;
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        return true;
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        // Basic standard card does nothing on play
    }
}
