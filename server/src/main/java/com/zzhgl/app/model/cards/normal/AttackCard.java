package com.zzhgl.app.model.cards.normal;

import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.AttackInteraction;
import com.zzhgl.app.networking.response.game.ResponseChampionStatsUpdateInteger;
import java.util.List;

/**
 * AttackCard represents a normal card used for attacking.
 */
public class AttackCard extends AbstractNormalCard {
    public AttackCard(int id, Suit suit, int value) {
        super(id, suit, value, NormalType.ATTACK);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds == null || targetIds.isEmpty()) return false;

        Champion casterChamp = caster.getSelectedChampion();
        if (casterChamp == null) return false;

        int maxTargets = 1 + casterChamp.getAdditionalTargetForAttack();
        if (targetIds.size() > maxTargets) return false;

        // Check attack limit
        if (casterChamp.getCurNumOfAttack() >= casterChamp.getMaxNumOfAttack()) {
            return false;
        }

        for (int targetId : targetIds) {
            if (caster.getID() == targetId) return false; // Cannot attack self

            Player target = game.getPlayerMap().get(targetId);
            if (target == null || target.getSelectedChampion() == null) return false;

            int distance = game.getDistance(caster.getID(), targetId);
            int effectiveDistance = distance + target.getSelectedChampion().getSpecialDefenseRange();
            
            if (effectiveDistance > casterChamp.getAttackRange()) {
                return false;
            }
        }
        return true;
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        if (targetIds == null || targetIds.isEmpty()) return;

        Champion casterChamp = caster.getSelectedChampion();
        if (casterChamp != null) {
            casterChamp.setCurNumOfAttack(casterChamp.getCurNumOfAttack() + 1);
            
            // Notify all players about the updated attack count
            ResponseChampionStatsUpdateInteger statsUpdate = new ResponseChampionStatsUpdateInteger(
                casterChamp.getId(),
                Champion.STAT_CUR_NUM_ATTACK,
                casterChamp.getCurNumOfAttack()
            );
            for (Player p : game.getPlayers()) {
                p.addResponseForUpdate(statsUpdate);
            }
        }

        for (int targetId : targetIds) {
            Player target = game.getPlayerMap().get(targetId);
            if (target != null) {
                int damage = casterChamp != null ? casterChamp.getAttack() : 1;
                int reqDef = casterChamp != null ? casterChamp.getRequiredDefenseAmount() : 1;
                AttackInteraction interaction = new AttackInteraction(caster, target, this, damage, reqDef);
                game.getInteractionStack().push(interaction);
            }
        }
    }
}
