package com.zzhgl.app.model.cards.normal;

import com.zzhgl.app.model.cards.AbstractNormalCard;

/**
 * AttackCard represents a normal card used for attacking.
 */
public class AttackCard extends AbstractNormalCard {
    public AttackCard(int id, Suit suit, int value) {
        super(id, suit, value, NormalType.ATTACK);
    }
}
