package com.zzhgl.app.model.cards.normal;

import com.zzhgl.app.model.cards.AbstractNormalCard;

/**
 * DodgeCard represents a normal card used for dodging attacks.
 */
public class DodgeCard extends AbstractNormalCard {
    public DodgeCard(int id, Suit suit, int value) {
        super(id, suit, value, NormalType.DODGE);
    }
}
