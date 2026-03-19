package com.zzhgl.app.model.cards.special;

import com.zzhgl.app.model.cards.AbstractSpecialCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.FireInteraction;
import java.util.List;

public class FireCard extends AbstractSpecialCard {
    public FireCard(int id, Suit suit, int value) {
        super(id, suit, value, SpecialType.FIRE);
    }

    @Override
    public boolean validate(GameManager game, Player caster, List<Integer> targetIds) {
        return game.getAlivePlayers().size() >= 2;
    }

    @Override
    public void play(GameManager game, Player caster, List<Integer> targetIds) {
        List<Player> targets = game.getAlivePlayersClockwise(caster);
        targets.remove(caster);
        
        // Push in reverse order so they resolve from the stack in clockwise order
        for (int i = targets.size() - 1; i >= 0; i--) {
            Player target = targets.get(i);
            game.getInteractionStack().push(new FireInteraction(caster, target, this));
        }
    }
}
