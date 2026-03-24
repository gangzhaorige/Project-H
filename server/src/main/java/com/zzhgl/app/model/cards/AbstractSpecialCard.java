package com.zzhgl.app.model.cards;

/**
 * AbstractSpecialCard represents specialized effects like Arrow, Duel, or Draw.
 */
public abstract class AbstractSpecialCard extends AbstractCard {
    public enum SpecialType {
        ARROW, DUEL, DRAW, STEAL, DISMANTLE, HEAL_ALL, FIRE, NEGATE, PRISON
    }

    protected SpecialType type;

    public AbstractSpecialCard(int id, Suit suit, int value, SpecialType type) {
        super(id, suit, value);
        this.type = type;
    }

    public SpecialType getSpecialType() { return type; }

    @Override
    public Category getCategory() {
        return Category.SPECIAL;
    }
}
