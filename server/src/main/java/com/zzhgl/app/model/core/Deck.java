package com.zzhgl.app.model.core;

import java.util.ArrayList;
import java.util.List;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.CardFactory;

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
        CardFactory factory = CardFactory.getInstance();
        for (AbstractCard.Suit suit : AbstractCard.Suit.values()) {
            for (int value = 1; value <= 13; value++) {
                // Alternating between Attack and Dodge for now
                AbstractNormalCard.NormalType type = (idCounter % 2 == 0) 
                    ? AbstractNormalCard.NormalType.ATTACK 
                    : AbstractNormalCard.NormalType.DODGE;
                
                cards.add(factory.createNormalCard(idCounter++, suit, value, type));
            }
        }
    }

    public List<AbstractCard> getCards() {
        return new ArrayList<>(cards);
    }
}
