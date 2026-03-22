package com.zzhgl.app.model.core;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.List;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractSpecialCard;
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
                int mod = idCounter % 10;
                if (mod < 3) { // 30% Attack
                    // cards.add(factory.createNormalCard(idCounter++, suit, value, AbstractNormalCard.NormalType.ATTACK));
                } else if (mod < 5) { // 20% Dodge
                    // cards.add(factory.createNormalCard(idCounter++, suit, value, AbstractNormalCard.NormalType.DODGE));
                } else if (mod == 5) { // 10% Heal
                    // cards.add(factory.createNormalCard(idCounter++, suit, value, AbstractNormalCard.NormalType.HEAL));
                } else if (mod == 6) { // 10% Draw
                    cards.add(factory.createSpecialCard(idCounter++, suit, value, AbstractSpecialCard.SpecialType.DRAW));
                } else if (mod == 7) { // 10% Duel
                    cards.add(factory.createSpecialCard(idCounter++, suit, value, AbstractSpecialCard.SpecialType.DUEL));
                } else if (mod == 8) { // 10% Fire
                    cards.add(factory.createSpecialCard(idCounter++, suit, value, AbstractSpecialCard.SpecialType.FIRE));
                } else if (mod == 9) { // 10% Negate
                    
                }
            }
        }
        
        // // Add a few rare special cards manually
        // cards.add(factory.createSpecialCard(idCounter++, AbstractCard.Suit.HEART, 1, AbstractSpecialCard.SpecialType.HEAL_ALL));
        // cards.add(factory.createSpecialCard(idCounter++, AbstractCard.Suit.SPADE, 1, AbstractSpecialCard.SpecialType.ARROW));
        // cards.add(factory.createSpecialCard(idCounter++, AbstractCard.Suit.SPADE, 6, AbstractSpecialCard.SpecialType.PRISON));
        // cards.add(factory.createSpecialCard(idCounter++, AbstractCard.Suit.CLUB, 6, AbstractSpecialCard.SpecialType.PRISON));
        for(int i = 0; i < 10; i++) {
            cards.add(factory.createSpecialCard(idCounter++, AbstractCard.Suit.DIAMOND, 6, AbstractSpecialCard.SpecialType.PRISON));
            cards.add(factory.createSpecialCard(idCounter++, AbstractCard.Suit.CLUB, 6, AbstractSpecialCard.SpecialType.PRISON));
            cards.add(factory.createSpecialCard(idCounter++, AbstractCard.Suit.HEART, 6, AbstractSpecialCard.SpecialType.PRISON));
            cards.add(factory.createSpecialCard(idCounter++, AbstractCard.Suit.SPADE, 6, AbstractSpecialCard.SpecialType.PRISON));
        }

        Collections.shuffle(cards);
        
    }

    public List<AbstractCard> getCards() {
        return new ArrayList<>(cards);
    }
}
