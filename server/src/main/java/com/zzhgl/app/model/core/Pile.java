package com.zzhgl.app.model.core;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Stack;
import com.zzhgl.app.model.cards.AbstractCard;

/**
 * Pile represents a collection of cards (e.g., draw pile, discard pile).
 */
public class Pile {
    private Stack<AbstractCard> cards;

    public Pile() {
        this.cards = new Stack<>();
    }

    public void addCard(AbstractCard card) {
        cards.push(card);
    }

    public void addCards(List<AbstractCard> newCards) {
        for (AbstractCard card : newCards) {
            cards.push(card);
        }
    }

    public AbstractCard draw() {
        if (cards.isEmpty()) return null;
        return cards.pop();
    }

    public void shuffle() {
        Collections.shuffle(cards);
    }

    public int size() {
        return cards.size();
    }

    public boolean isEmpty() {
        return cards.isEmpty();
    }

    public List<AbstractCard> clear() {
        List<AbstractCard> removed = new ArrayList<>(cards);
        cards.clear();
        return removed;
    }
}
