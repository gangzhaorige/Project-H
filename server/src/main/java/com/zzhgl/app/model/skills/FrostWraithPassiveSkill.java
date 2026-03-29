package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

import java.util.Collections;
import java.util.List;

/**
 * Skill: Transcendent Flash (JingLiu). Passive.
 * When JingLiu plays an attack card, the required amount of defense cards 
 * the enemy must play becomes 2.
 */
public class FrostWraithPassiveSkill extends AbstractSkill {

    public FrostWraithPassiveSkill() {
        super(12, "Frost Wraith", false); // ID 12, Passive
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.BEFORE_CARD_PLAYED;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        if (event.getType() != GameEvent.EventType.BEFORE_CARD_PLAYED) return false;
        
        Player player = (Player) event.getParam("player");
        Object card = event.getParam("card");
        
        // Trigger if the owner is playing an ATTACK card
        return owner.equals(player) && 
               card instanceof AbstractNormalCard normal && 
               normal.getNormalType() == AbstractNormalCard.NormalType.ATTACK;
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        if (owner.getSelectedChampion() != null) {
            Log.printf("JingLiu Passive (Transcendent Flash) triggered! Required defense cards: 2.");
            owner.getSelectedChampion().setRequiredDefenseAmount(2);
        }
        return Collections.emptyList();
    }
}
