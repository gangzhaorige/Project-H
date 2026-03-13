package com.zzhgl.app.model.core;

import java.util.ArrayList;
import java.util.List;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.StandardCard;

/**
 * Deck initializes the standard set of cards for the game.
 */
public class Deck {
    private List<AbstractCard> cards;

    public Deck() {
        this.cards = new ArrayList<>();
        initialize();
    }

    private void initialize() {
        int idCounter = 1;
        for (AbstractCard.Suit suit : AbstractCard.Suit.values()) {
            for (int value = 1; value <= 13; value++) {
                cards.add(new StandardCard(idCounter++, suit, value));
            }
        }
    }

    public List<AbstractCard> getCards() {
        return new ArrayList<>(cards);
    }
}
