package com.zzhgl.app.model.cards;

/**
 * AbstractCard represents a basic card in the game.
 */
public abstract class AbstractCard {
    public enum Suit {
        SPADE, DIAMOND, HEART, CLUB
    }

    public enum Category {
        NORMAL, SPECIAL, EQUIPMENT, STANDARD
    }

    protected int id;
    protected Suit suit;
    protected int value; // 1-13

    public AbstractCard(int id, Suit suit, int value) {
        this.id = id;
        this.suit = suit;
        this.value = value;
    }

    public int getId() { return id; }
    public Suit getSuit() { return suit; }
    public int getValue() { return value; }

    public abstract Category getCategory();

    @Override
    public String toString() {
        return String.format("%s[%d of %s]", getClass().getSimpleName(), value, suit);
    }
}
