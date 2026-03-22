package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.JudgementOverrideAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

import java.util.List;
import java.util.Optional;

/**
 * Skill: JudgementOverride. Allows the player to play a card from hand to replace the current judgement card.
 */
public class JudgementOverrideSkill extends AbstractSkill {

    public JudgementOverrideSkill() {
        super(4, "Celestial Alteration", true); // Optional skill
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.BEFORE_JUDGEMENT_RESOLVE;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        return event.getType() == GameEvent.EventType.BEFORE_JUDGEMENT_RESOLVE && owner.getHand().size() > 0;
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        // Step 1: Player accepted but hasn't picked a card yet
        if (data == null) {
            return null; // Signals SkillResolutionState to wait for further input
        }

        // Step 2: Player picked a card (Integer cardId)
        if (data instanceof Integer cardId) {
            Optional<AbstractCard> cardOpt = owner.getHand().getCards().stream()
                    .filter(c -> c.getId() == cardId)
                    .findFirst();

            if (cardOpt.isPresent()) {
                // Return a single atomic action that performs the override
                return List.of(new JudgementOverrideAction(owner, event, cardOpt.get()));
            }
        }
        return null;
    }

    @Override
    public void onTimeout(GameManager game, Player owner, GameEvent event) {
        List<AbstractCard> cards = owner.getHand().getCards();
        if (!cards.isEmpty()) {
            AbstractCard lastCard = cards.get(cards.size() - 1);
            Log.printf("JudgementOverrideSkill timed out. Auto-using LAST card.");
            game.getActionQueue().add(new JudgementOverrideAction(owner, event, lastCard));
        }
    }
}
