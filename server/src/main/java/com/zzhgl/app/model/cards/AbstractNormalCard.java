package com.zzhgl.app.model.cards;

import com.zzhgl.app.model.interactions.AbstractInteraction;

import java.util.ArrayList;
import java.util.List;

/**
 * AbstractNormalCard represents common actions like Attack, Dodge, or Heal.
 */
public abstract class AbstractNormalCard extends AbstractCard {
    public enum NormalType {
        ATTACK, DODGE, HEAL
    }

    protected NormalType type;

    public AbstractNormalCard(int id, Suit suit, int value, NormalType type) {
        super(id, suit, value);
        this.type = type;
    }

    public NormalType getNormalType() { return type; }

    @Override
    public Category getCategory() {
        return Category.NORMAL;
    }

    /**
     * Plays the card and generates a list of interactions.
     */
    public List<AbstractInteraction> play() {
        return new ArrayList<>(); // Empty for now
    }
}
