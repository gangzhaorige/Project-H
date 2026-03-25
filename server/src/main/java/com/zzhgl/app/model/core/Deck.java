package com.zzhgl.app.model.core;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.cards.CardFactory;

/**
 * Deck initializes a balanced set of cards for the game.
 * Total: 108 cards.
 */
public class Deck {
    private List<AbstractCard> cards;
    private int idCounter = 1;

    public Deck() {
        this.cards = new ArrayList<>();
        initialize();
    }

    private void initialize() {
        CardFactory factory = CardFactory.getInstance();
        
        // Distribution for ~108 cards
        // addNormalCards(factory, AbstractNormalCard.NormalType.ATTACK, 30);
        // addNormalCards(factory, AbstractNormalCard.NormalType.DODGE, 20);
        // addNormalCards(factory, AbstractNormalCard.NormalType.HEAL, 8);
        
        // addSpecialCards(factory, AbstractSpecialCard.SpecialType.NEGATE, 10);
        // addSpecialCards(factory, AbstractSpecialCard.SpecialType.DRAW, 8);
        // addSpecialCards(factory, AbstractSpecialCard.SpecialType.STEAL, 20);
        addSpecialCards(factory, AbstractSpecialCard.SpecialType.DISMANTLE, 20);
        // addSpecialCards(factory, AbstractSpecialCard.SpecialType.DUEL, 6);
        // addSpecialCards(factory, AbstractSpecialCard.SpecialType.FIRE, 6);
        
        // addSpecialCards(factory, AbstractSpecialCard.SpecialType.PRISON, 4);
        // addSpecialCards(factory, AbstractSpecialCard.SpecialType.ARROW, 2);
        // addSpecialCards(factory, AbstractSpecialCard.SpecialType.HEAL_ALL, 2);

        Collections.shuffle(cards);
    }

    private void addNormalCards(CardFactory factory, AbstractNormalCard.NormalType type, int count) {
        for (int i = 0; i < count; i++) {
            AbstractCard.Suit suit = AbstractCard.Suit.values()[i % 4];
            int value = (i % 13) + 1;
            cards.add(factory.createNormalCard(idCounter++, suit, value, type));
        }
    }

    private void addSpecialCards(CardFactory factory, AbstractSpecialCard.SpecialType type, int count) {
        for (int i = 0; i < count; i++) {
            AbstractCard.Suit suit = AbstractCard.Suit.values()[i % 4];
            int value = (i % 13) + 1;
            cards.add(factory.createSpecialCard(idCounter++, suit, value, type));
        }
    }

    public List<AbstractCard> getCards() {
        return new ArrayList<>(cards);
    }
}
