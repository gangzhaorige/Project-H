package com.zzhgl.app.model.cards.normal;

import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.HealInteraction;
import java.util.List;

/**
 * HealCard represents a normal card used for healing.
 */
public class HealCard extends AbstractNormalCard {
    public HealCard(int id, Suit suit, int value) {
        super(id, suit, value, NormalType.HEAL);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        Champion champ = caster.getSelectedChampion();
        if (champ == null) return false;
        return champ.getCurHP() < champ.getMaxHP();
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        HealInteraction interaction = new HealInteraction(caster, caster, this, 1, false);
        game.getInteractionStack().push(interaction);
    }
}

