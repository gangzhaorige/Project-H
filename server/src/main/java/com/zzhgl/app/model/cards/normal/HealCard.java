package com.zzhgl.app.model.cards.normal;

import com.zzhgl.app.model.cards.AbstractNormalCard;

/**
 * HealCard represents a normal card used for healing.
 */
public class HealCard extends AbstractNormalCard {
    public HealCard(int id, Suit suit, int value) {
        super(id, suit, value, NormalType.HEAL);
    }
}
