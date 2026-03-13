package com.zzhgl.app.model.cards;

import com.zzhgl.app.model.champions.Champion;

/**
 * AbstractEquipmentCard provides additional stats or skills to a champion when equipped.
 */
public abstract class AbstractEquipmentCard extends AbstractCard {
    
    public AbstractEquipmentCard(int id, Suit suit, int value) {
        super(id, suit, value);
    }

    @Override
    public Category getCategory() {
        return Category.EQUIPMENT;
    }

    /**
     * Applies the equipment's effects to the champion.
     */
    public abstract void onEquip(Champion champion);

    /**
     * Removes the equipment's effects from the champion.
     */
    public abstract void onRemove(Champion champion);
}
