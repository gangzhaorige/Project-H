package com.zzhgl.app.model.cards;

/**
 * AbstractSpecialCard represents specialized effects like Arrow, Duel, or Draw.
 */
public abstract class AbstractSpecialCard extends AbstractCard {
    public enum SpecialType {
        ARROW(101), 
        DUEL(102), 
        DRAW(103), 
        STEAL(104), 
        DISMANTLE(105), 
        HEAL_ALL(106), 
        FIRE(107), 
        NEGATE(108), 
        PRISON(109), 
        DRAW_SKIP(110);

        private final int id;
        SpecialType(int id) { this.id = id; }
        public int getId() { return id; }
    }

    protected SpecialType type;

    public AbstractSpecialCard(int id, Suit suit, int value, SpecialType type) {
        super(id, suit, value);
        this.type = type;
    }

    public SpecialType getSpecialType() { return type; }

    @Override
    public int getEnumId() {
        return type.getId();
    }

    @Override
    public Category getCategory() {
        return Category.SPECIAL;
    }
}
