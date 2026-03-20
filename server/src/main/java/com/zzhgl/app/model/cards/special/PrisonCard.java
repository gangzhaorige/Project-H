package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.PrisonInteraction;
import java.util.List;

public class PrisonCard extends AbstractSpecialCard {
    public PrisonCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.PRISON);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds == null || targetIds.size() != 1) return false;
        int targetId = targetIds.get(0);
        if (targetId == caster.getID()) return false;
        
        Player target = game.getPlayerMap().get(targetId);
        if (target == null) return false;
        
        // Cannot imprison someone who already has prison
        return target.getActiveEffects().stream()
                .noneMatch(e -> e.getName().equals("Prison"));
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds != null && !targetIds.isEmpty()) {
            Player target = game.getPlayerMap().get(targetIds.get(0));
            if (target != null) {
                game.getInteractionStack().push(new PrisonInteraction(caster, target, this));
            }
        }
    }
}
