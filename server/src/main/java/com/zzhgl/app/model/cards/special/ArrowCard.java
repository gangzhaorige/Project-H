package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.ArrowInteraction;
import java.util.List;

/**
 * ArrowCard represents a special card that attacks all other players.
 */
public class ArrowCard extends AbstractSpecialCard {
    public ArrowCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.ARROW);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        int aliveCount = 0;
        for (Player p : game.getPlayers()) {
            if (p.getSelectedChampion() != null && p.getSelectedChampion().getCurHP() > 0) {
                aliveCount++;
            }
        }
        return aliveCount >= 2;
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        List<Player> targets = game.getAlivePlayersClockwise(caster);
        targets.remove(caster);
        for (int i = targets.size() - 1; i >= 0; i--) {
            Player target = targets.get(i);
            game.getInteractionStack().push(new ArrowInteraction(caster, target, this));
        }
    }
}

