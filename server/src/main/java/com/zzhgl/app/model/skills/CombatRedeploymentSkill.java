package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.CombatRedeploymentInteraction;
import com.zzhgl.app.networking.response.game.ResponseDiscardCard;
import com.zzhgl.app.networking.response.game.ResponseSkillActivated;
import com.zzhgl.app.networking.response.game.ResponseUpdatePlayerOrder;
import com.zzhgl.app.utility.Log;

import java.util.Collections;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

/**
 * Skill: Combat Redeployment (Bronya). Active.
 * Discard 2 cards, target 1 champion, switch position with Bronya's position + 1.
 * Once per turn.
 */
public class CombatRedeploymentSkill extends AbstractSkill {
    private int lastTurnUsed = -1;

    public CombatRedeploymentSkill() {
        super(10, "Combat Redeployment", false);
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.SKILL_ACTIVATED; // Unused for manual active skills
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        return false; // Active skills use the activate method
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        return Collections.emptyList();
    }
    
    @Override
    public boolean validateActivation(GameManager game, Player owner, List<Integer> discardCardIds, List<Integer> targetIds) {
        if (lastTurnUsed == game.getTurnCounter()) {
            Log.printf_e("Player %d tried to use Combat Redeployment again this turn!", owner.getID());
            return false;
        }
        if (discardCardIds.size() != 2) {
            Log.printf_e("Combat Redeployment requires exactly 2 cards discarded.");
            return false;
        }
        if (targetIds.size() != 1) return false;
        
        // Verify discards exist in hand
        for (int cardId : discardCardIds) {
            if (owner.getHand().getCards().stream().noneMatch(c -> c.getId() == cardId)) return false;
        }
        return true;
    }

    @Override
    public void activate(GameManager game, Player owner, List<Integer> discardCardIds, List<Integer> targetIds) {
        Player targetPlayer = game.getPlayerMap().get(targetIds.get(0));
        if (targetPlayer != null) {
            game.getInteractionStack().push(new CombatRedeploymentInteraction(owner, targetPlayer, this, discardCardIds));
        }
    }

    public void setUsed(int turn) {
        this.lastTurnUsed = turn;
    }
    
    public int getLastTurnUsed() {
        return lastTurnUsed;
    }
}
