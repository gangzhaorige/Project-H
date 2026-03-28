package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.DrawCardAction;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import java.util.List;

/**
 * Skill 3: On Last Card Played: Draw 2 cards (Automatic).
 */
public class DrawOnLastCardSkill extends AbstractSkill {
    
    public DrawOnLastCardSkill() {
        super(3, "Final Spark", false);
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.CARD_PLAYED;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        // Triggered after a card is played by the owner if their hand is now empty
        if (event.getType() != GameEvent.EventType.CARD_PLAYED) return false;
        
        Player cardPlayer = (Player) event.getParam("player");
        return cardPlayer != null && cardPlayer.equals(owner) && owner.getHand().size() == 0;
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        return List.of(new DrawCardAction(owner, 2));
    }
}
