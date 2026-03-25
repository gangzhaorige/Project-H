package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.StealInteraction;
import com.zzhgl.app.utility.Log;

import java.util.List;

/**
 * StealCard allows the player to steal a card from a neighbor's hand.
 */
public class StealCard extends AbstractSpecialCard {
    public StealCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.STEAL);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds == null || targetIds.size() != 1) {
            Log.printf("StealCard Validation Failed: Invalid targetIds size. Caster=%d, TargetIds.size()=%d", caster.getID(), targetIds == null ? -1 : targetIds.size());
            return false;
        }
        int targetId = targetIds.get(0);

        Player target = game.getPlayerMap().get(targetId);
        if (target == null) {
            Log.printf("StealCard Validation Failed: Target player not found. Caster=%d, TargetId=%d", caster.getID(), targetId);
            return false;
        }
        if (target.getID() == caster.getID()) {
            Log.printf("StealCard Validation Failed: Cannot target self. Caster=%d, TargetId=%d", caster.getID(), target.getID());
            return false;
        }
        if (target.getHand().getCards().isEmpty()) {
            Log.printf("StealCard Validation Failed: Target hand is empty. Caster=%d, TargetId=%d", caster.getID(), target.getID());
            return false;
        }

        // Range check using special range and special defense
        // StealCard has an inherent +1 range allowing it to reach neighbors even if specialRange is 0.
        int distance = game.getDistance(caster.getID(), target.getID());
        int effectiveDistance = distance + target.getSelectedChampion().getSpecialDefenseRange();
        boolean canSteal = effectiveDistance <= caster.getSelectedChampion().getSpecialRange() + 1;

        Log.printf("StealCard Validation: Caster=%d, Target=%d, CasterSpecialRange=%d, TargetSpecialDefense=%d, Distance=%d, EffectiveDistance=%d, CanSteal=%b",
                   caster.getID(), target.getID(), caster.getSelectedChampion().getSpecialRange(),
                   target.getSelectedChampion().getSpecialDefenseRange(), distance, effectiveDistance, canSteal);

        return canSteal;
        }
    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds != null && !targetIds.isEmpty()) {
            Player target = game.getPlayerMap().get(targetIds.get(0));
            if (target != null) {
                game.getInteractionStack().push(new StealInteraction(caster, target, this));
            }
        }
    }
}
