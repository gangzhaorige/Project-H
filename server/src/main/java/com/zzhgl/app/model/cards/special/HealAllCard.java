package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.HealInteraction;
import java.util.List;

/**
 * HealAllCard represents a special card that heals all players.
 */
public class HealAllCard extends AbstractSpecialCard {
    public HealAllCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.HEAL_ALL);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        return true; 
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        List<Player> targets = game.getAlivePlayersClockwise(caster);
        
        // Push in reverse order so they resolve from the stack in clockwise order
        for (int i = targets.size() - 1; i >= 0; i--) {
            Player target = targets.get(i);
            // Only heal players who need it
            if (target.getSelectedChampion() != null && target.getSelectedChampion().getCurHP() < target.getSelectedChampion().getMaxHP()) {
                game.getInteractionStack().push(new HealInteraction(caster, target, this, 1, true));
            }
        }
    }
}
