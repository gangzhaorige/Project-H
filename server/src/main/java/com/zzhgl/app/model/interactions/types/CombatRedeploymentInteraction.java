package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.skills.CombatRedeploymentSkill;
import com.zzhgl.app.networking.response.game.ResponseDiscardCard;
import com.zzhgl.app.networking.response.game.ResponseSkillActivated;
import com.zzhgl.app.networking.response.game.ResponseUpdatePlayerOrder;
import com.zzhgl.app.utility.Log;

import java.util.Collections;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

public class CombatRedeploymentInteraction extends AbstractInteraction {
    private final CombatRedeploymentSkill skill;
    private final List<Integer> discardCardIds;

    public CombatRedeploymentInteraction(Player caster, Player target, CombatRedeploymentSkill skill, List<Integer> discardCardIds) {
        super(caster, target, null, false); 
        this.skill = skill;
        this.discardCardIds = discardCardIds;
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating CombatRedeploymentInteraction from %s targeting %s.", caster.getUsername(), target.getUsername());
        
        // a. Discard cards
        for (int cardId : discardCardIds) {
            Optional<AbstractCard> cardToDiscard = caster.getHand().getCards().stream().filter(c -> c.getId() == cardId).findFirst();
            if (cardToDiscard.isPresent()) {
                AbstractCard c = cardToDiscard.get();
                caster.getHand().removeCard(c);
                game.getDiscardPile().addCard(c);
                
                // Broadcast discard
                ResponseDiscardCard discardRes = new ResponseDiscardCard(caster.getID(), c, false, false);
                for (Player p : game.getPlayers()) p.addResponseForUpdate(discardRes);
            }
        }

        // b. Swap positions
        int bronyaIdx = game.getPlayers().indexOf(caster);
        int swapTargetIdx = (bronyaIdx + 1) % game.getPlayers().size();
        
        int actualTargetIdx = game.getPlayers().indexOf(target);
        if (actualTargetIdx != -1) {
            Collections.swap(game.getPlayers(), swapTargetIdx, actualTargetIdx);
            
            // Re-calculate activePlayerIndex
            game.setActivePlayerIndex(game.getPlayers().indexOf(caster));

            // c. Notify everyone
            List<Integer> newOrder = game.getPlayers().stream().map(Player::getID).collect(Collectors.toList());
            ResponseUpdatePlayerOrder orderRes = new ResponseUpdatePlayerOrder(newOrder);
            for (Player p : game.getPlayers()) p.addResponseForUpdate(orderRes);
            
            // Skill activation visual/audio
            int skillIndex = caster.getSelectedChampion().getSkills().indexOf(skill);
            ResponseSkillActivated skillRes = new ResponseSkillActivated(caster.getID(), skillIndex);
            for (Player p : game.getPlayers()) p.addResponseForUpdate(skillRes);
            
            skill.setUsed(game.getTurnCounter());
            Log.printf("Player %d used Combat Redeployment! New player order: %s", caster.getID(), newOrder);
        }
    }
}
