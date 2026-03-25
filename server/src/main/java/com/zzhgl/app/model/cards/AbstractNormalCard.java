package com.zzhgl.app.model.cards;

/**
 * AbstractNormalCard represents common actions like Attack, Dodge, or Heal.
 */
public abstract class AbstractNormalCard extends AbstractCard {
    public enum NormalType {
        ATTACK(1), 
        DODGE(2), 
        HEAL(3);

        private final int id;
        NormalType(int id) { this.id = id; }
        public int getId() { return id; }
    }

    protected NormalType type;

    public AbstractNormalCard(int id, Suit suit, int value, NormalType type) {
        super(id, suit, value);
        this.type = type;
    }

    public NormalType getNormalType() { return type; }

    @Override
    public int getEnumId() {
        return type.getId();
    }

    @Override
    public Category getCategory() {
        return Category.NORMAL;
    }
}
