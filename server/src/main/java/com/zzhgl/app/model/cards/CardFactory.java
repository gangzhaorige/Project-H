package com.zzhgl.app.model.cards;

import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.cards.normal.*;
import com.zzhgl.app.model.cards.special.*;

/**
 * CardFactory handles the creation of various card types.
 */
public class CardFactory {
    private static CardFactory instance;

    private CardFactory() {}

    public static CardFactory getInstance() {
        if (instance == null) {
            instance = new CardFactory();
        }
        return instance;
    }

    /**
     * Creates a Normal Card (Attack, Dodge, Heal).
     */
    public AbstractNormalCard createNormalCard(int id, AbstractCard.Suit suit, int value, AbstractNormalCard.NormalType type) {
        switch (type) {
            case ATTACK: return new AttackCard(id, suit, value);
            case DODGE: return new DodgeCard(id, suit, value);
            case HEAL: return new HealCard(id, suit, value);
            default: return null;
        }
    }

    /**
     * Creates a Special Card (Arrow, Duel, etc).
     */
    public AbstractSpecialCard createSpecialCard(int id, AbstractCard.Suit suit, int value, AbstractSpecialCard.SpecialType type) {
        switch (type) {
            case ARROW: return new ArrowCard(id, suit, value);
            case HEAL_ALL: return new HealAllCard(id, suit, value);
            case DRAW: return new DrawCard(id, suit, value);
            // Default to an anonymous class for types not yet explicitly handled with a concrete class
            default: return new AbstractSpecialCard(id, suit, value, type) {};
        }
    }

    /**
     * Creates an Equipment Card.
     */
    public AbstractEquipmentCard createEquipmentCard(int id, AbstractCard.Suit suit, int value) {
        return new AbstractEquipmentCard(id, suit, value) {
            @Override
            public void onEquip(Champion champion) {
                // Implementation for specific equipment
            }

            @Override
            public void onRemove(Champion champion) {
                // Implementation for specific equipment
            }
        };
    }
}
