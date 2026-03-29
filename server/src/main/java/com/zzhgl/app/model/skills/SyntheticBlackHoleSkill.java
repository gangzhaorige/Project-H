package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.types.SyntheticBlackHoleInteraction;
import com.zzhgl.app.utility.Log;

import java.util.Collections;
import java.util.List;

/**
 * Skill: Synthetic Black Hole (Welt). Active.
 * Player discards cards from their hand and draws the same number of cards.
 * Once per turn.
 */
public class SyntheticBlackHoleSkill extends AbstractSkill {
    private int lastTurnUsed = -1;

    public SyntheticBlackHoleSkill() {
        super(11, "Synthetic Black Hole", false);
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
            Log.printf_e("Player %d tried to use Synthetic Black Hole again this turn!", owner.getID());
            return false;
        }
        if (discardCardIds.isEmpty()) {
            Log.printf_e("Synthetic Black Hole requires at least 1 card to be discarded.");
            return false;
        }
        
        // Verify discards exist in hand
        for (int cardId : discardCardIds) {
            if (owner.getHand().getCards().stream().noneMatch(c -> c.getId() == cardId)) return false;
        }
        return true;
    }

    @Override
    public void activate(GameManager game, Player owner, List<Integer> discardCardIds, List<Integer> targetIds) {
        game.getInteractionStack().push(new SyntheticBlackHoleInteraction(owner, this, discardCardIds));
    }

    public void setUsed(int turn) {
        this.lastTurnUsed = turn;
    }
}
