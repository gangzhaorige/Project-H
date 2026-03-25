package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.DismantleInteraction;
import com.zzhgl.app.utility.Log;

import java.util.List;

/**
 * DismantleCard (Destroy) allows the player to remove a card from any player's hand.
 * Can be played from any range.
 */
public class DismantleCard extends AbstractSpecialCard {
    public DismantleCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.DISMANTLE);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds == null || targetIds.size() != 1) {
            Log.printf("DismantleCard Validation Failed: Invalid targetIds size. Caster=%d", caster.getID());
            return false;
        }
        int targetId = targetIds.get(0);

        Player target = game.getPlayerMap().get(targetId);
        if (target == null) {
            Log.printf("DismantleCard Validation Failed: Target player not found. Caster=%d, TargetId=%d", caster.getID(), targetId);
            return false;
        }
        if (target.getID() == caster.getID()) {
            Log.printf("DismantleCard Validation Failed: Cannot target self. Caster=%d", caster.getID());
            return false;
        }
        if (target.getHand().getCards().isEmpty()) {
            Log.printf("DismantleCard Validation Failed: Target hand is empty. Caster=%d, TargetId=%d", caster.getID(), target.getID());
            return false;
        }

        // Dismantle has no range restriction (can be played from any range)
        return true;
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds != null && !targetIds.isEmpty()) {
            Player target = game.getPlayerMap().get(targetIds.get(0));
            if (target != null) {
                game.getInteractionStack().push(new DismantleInteraction(caster, target, this));
            }
        }
    }
}
