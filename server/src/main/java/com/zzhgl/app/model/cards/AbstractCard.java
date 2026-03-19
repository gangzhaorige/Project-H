package com.zzhgl.app.model.cards;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import java.util.List;

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

    /**
     * Validates if the card can be played by the caster against the specified targets.
     */
    public abstract boolean validate(GameManager game, Player caster, List<Integer> targetIds);

    public abstract void play(GameManager game, Player caster, List<Integer> targetIds);

    @Override
    public String toString() {
        return String.format("%s[%d of %s]", getClass().getSimpleName(), value, suit);
    }
}
