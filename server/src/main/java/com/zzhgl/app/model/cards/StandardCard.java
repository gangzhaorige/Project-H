package com.zzhgl.app.model.cards;

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
}
