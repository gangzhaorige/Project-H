package com.zzhgl.app.model.core;

import java.util.ArrayList;
import java.util.List;
import com.zzhgl.app.model.cards.AbstractCard;

/**
 * Hand represents a player's hand of cards.
 */
public class Hand {
    private List<AbstractCard> cards;

    public Hand() {
        this.cards = new ArrayList<>();
    }

    public void addCard(AbstractCard card) {
        cards.add(card);
    }

    public void removeCard(AbstractCard card) {
        cards.remove(card);
    }

    public boolean removeCardById(int id) {
        return cards.removeIf(card -> card.getId() == id);
    }

    public List<AbstractCard> getCards() {
        return new ArrayList<>(cards);
    }

    public int size() {
        return cards.size();
    }

    public void clear() {
        cards.clear();
    }
}
