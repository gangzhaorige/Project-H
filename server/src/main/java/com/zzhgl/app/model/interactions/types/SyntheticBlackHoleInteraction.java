package com.zzhgl.app.model.interactions.types;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.AbstractInteraction;
import com.zzhgl.app.model.skills.SyntheticBlackHoleSkill;
import com.zzhgl.app.networking.response.game.ResponseDiscardCard;
import com.zzhgl.app.networking.response.game.ResponseSkillActivated;
import com.zzhgl.app.utility.Log;

import java.util.List;
import java.util.Optional;

public class SyntheticBlackHoleInteraction extends AbstractInteraction {
    private final SyntheticBlackHoleSkill skill;
    private final List<Integer> discardCardIds;

    public SyntheticBlackHoleInteraction(Player owner, SyntheticBlackHoleSkill skill, List<Integer> discardCardIds) {
        // Self-targeted, negatable
        super(owner, owner, null, false);
        this.skill = skill;
        this.discardCardIds = discardCardIds;
    }

    @Override
    public void evaluate(GameManager game) {
        Log.printf("Evaluating SyntheticBlackHoleInteraction from %s.", caster.getUsername());
        
        int discardCount = 0;
        // 1. Discard cards
        for (int cardId : discardCardIds) {
            Optional<AbstractCard> cardToDiscard = caster.getHand().getCards().stream()
                .filter(c -> c.getId() == cardId).findFirst();
            if (cardToDiscard.isPresent()) {
                AbstractCard c = cardToDiscard.get();
                caster.getHand().removeCard(c);
                game.getDiscardPile().addCard(c);
                discardCount++;
                
                // Broadcast discard
                ResponseDiscardCard discardRes = new ResponseDiscardCard(caster.getID(), c, false, false);
                for (Player p : game.getPlayers()) p.addResponseForUpdate(discardRes);
            }
        }

        // 2. Draw cards based on discarded amount
        if (discardCount > 0) {
            game.drawCards(caster, discardCount);
            Log.printf("Player %d used Synthetic Black Hole! Discarded %d, drew %d.", caster.getID(), discardCount, discardCount);
        }

        // 3. Notify everyone about skill activation
        int skillIndex = caster.getSelectedChampion().getSkills().indexOf(skill);
        ResponseSkillActivated skillRes = new ResponseSkillActivated(caster.getID(), skillIndex);
        for (Player p : game.getPlayers()) p.addResponseForUpdate(skillRes);
        
        skill.setUsed(game.getTurnCounter());
    }
}
