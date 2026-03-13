package com.zzhgl.app.model.cards;

import com.zzhgl.app.model.interactions.Interaction;

import java.util.ArrayList;
import java.util.List;

/**
 * AbstractSpecialCard represents specialized effects like Arrow, Duel, or Draw.
 */
public abstract class AbstractSpecialCard extends AbstractCard {
    public enum SpecialType {
        ARROW, DUEL, DRAW, STEAL, DISMANTLE, HEAL_ALL
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

    /**
     * Plays the card and generates a list of interactions.
     */
    public List<Interaction> play() {
        return new ArrayList<>(); // Empty for now
    }
}
