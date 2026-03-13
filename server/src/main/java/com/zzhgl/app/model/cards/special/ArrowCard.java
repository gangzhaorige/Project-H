package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;

/**
 * ArrowCard represents a special card that targets other players with arrows.
 */
public class ArrowCard extends AbstractSpecialCard {
    public ArrowCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.ARROW);
    }
}
