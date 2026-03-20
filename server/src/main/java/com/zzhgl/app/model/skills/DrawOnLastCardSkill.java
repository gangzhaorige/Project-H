package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;

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
        // Triggered after a card is played if the hand is now empty
        return event.getType() == GameEvent.EventType.CARD_PLAYED && owner.getHand().size() == 0;
    }

    @Override
    public boolean execute(GameManager game, Player owner, GameEvent event, Object data) {
        game.drawCards(owner, 2);
        return true;
    }
}
