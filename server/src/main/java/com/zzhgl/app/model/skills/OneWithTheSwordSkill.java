package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.UpdateChampionStatAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.cards.AbstractNormalCard;
import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

import java.util.List;

/**
 * Skill: One With the Sword (Yanqing). Passive.
 * On Attack card played, do not consume the curNumOfAttack.
 */
public class OneWithTheSwordSkill extends AbstractSkill {

    public OneWithTheSwordSkill() {
        super(8, "One With the Sword", false); // ID 8, Not optional
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.DAMAGE_TAKEN;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        if (event.getType() != GameEvent.EventType.DAMAGE_TAKEN) return false;

        Player source = (Player) event.getParam("source");
        AbstractCard card = (AbstractCard) event.getParam("card");
        Object interactionType = event.getParam("sourceInteractionType");
        Object requiredType = event.getParam("requiredType");

        // Logic: Yanqing dealt damage...
        if (owner.equals(source) && 
            // ...with an Attack card...
            card instanceof AbstractNormalCard normal && normal.getNormalType() == AbstractNormalCard.NormalType.ATTACK &&
            // ...specifically during an AttackInteraction...
            "AttackInteraction".equals(interactionType) &&
            // ...where the defender was prompted to play a Dodge card.
            AbstractNormalCard.NormalType.DODGE.equals(requiredType)) {
            return true;
        }
        return false;
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        Champion champ = owner.getSelectedChampion();
        if (champ != null) {
            Log.printf("Yanqing (Player %d) uses passive: %s! Attack count preserved because damage landed.", owner.getID(), getName());
            
            // Decrease attack count by 1 (reverting the increment from playing the card)
            int newValue = Math.max(0, champ.getCurNumOfAttack() - 1);
            
            return List.of(
                new UpdateChampionStatAction(owner, Champion.STAT_CUR_NUM_ATTACK, newValue, Champion::setCurNumOfAttack)
            );
        }
        return List.of();
    }
}
