package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.DrawSkipInteraction;
import java.util.List;

/**
 * DrawSkipCard skips the target's next draw phase if judgement fails.
 */
public class DrawSkipCard extends AbstractSpecialCard {
    public DrawSkipCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.DRAW_SKIP);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds == null || targetIds.size() != 1) return false;
        int targetId = targetIds.get(0);
        if (targetId == caster.getID()) return false;
        
        Player target = game.getPlayerMap().get(targetId);
        if (target == null) return false;
        
        // Cannot stack same effect
        return target.getActiveEffects().stream()
                .noneMatch(e -> e.getName().equals("DrawSkip"));
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds != null && !targetIds.isEmpty()) {
            Player target = game.getPlayerMap().get(targetIds.get(0));
            if (target != null) {
                game.getInteractionStack().push(new DrawSkipInteraction(caster, target, this));
            }
        }
    }
}
