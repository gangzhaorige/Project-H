package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;

/**
 * DrawCard represents a special card that allows the player to draw more cards.
 */
public class DrawCard extends AbstractSpecialCard {
    public DrawCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.DRAW);
    }
}
