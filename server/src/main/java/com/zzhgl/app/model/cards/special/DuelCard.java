package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.DuelInteraction;
import java.util.List;

public class DuelCard extends AbstractSpecialCard {
    public DuelCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.DUEL);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds == null || targetIds.isEmpty() || targetIds.size() > 1) return false;
        return targetIds.get(0) != caster.getID(); // Cannot duel self
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds != null && !targetIds.isEmpty()) {
            Player target = game.getPlayerMap().get(targetIds.get(0));
            if (target != null) {
                // Determine damage, could be 1 or based on champion
                int damage = 1; 
                game.getInteractionStack().push(new DuelInteraction(caster, target, this, damage));
            }
        }
    }
}
