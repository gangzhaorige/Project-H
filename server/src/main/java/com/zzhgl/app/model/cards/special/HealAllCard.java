package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;

/**
 * HealAllCard represents a special card that heals all allies.
 */
public class HealAllCard extends AbstractSpecialCard {
    public HealAllCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.HEAL_ALL);
    }
}
