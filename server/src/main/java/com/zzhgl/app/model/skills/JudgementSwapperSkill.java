package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.JudgementSwapperAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.utility.Log;

import java.util.List;
import java.util.Optional;

/**
 * Skill: JudgementSwapper. Allows the player to swap a SPADE or CLUB card from their hand 
 * with the current judgement card.
 */
public class JudgementSwapperSkill extends AbstractSkill {

    public JudgementSwapperSkill() {
        super(5, "Shadow Exchange", true); // Optional skill
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.BEFORE_JUDGEMENT_RESOLVE;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        if (event.getType() != GameEvent.EventType.BEFORE_JUDGEMENT_RESOLVE) return false;
        
        // Condition: Player must have at least one SPADE or CLUB in hand
        return owner.getHand().getCards().stream()
                .anyMatch(c -> c.getSuit() == AbstractCard.Suit.SPADE || c.getSuit() == AbstractCard.Suit.CLUB);
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        // Step 1: Player accepted the skill
        if (data == null) {
            return null; // Wait for card input
        }

        // Step 2: Player picked a card
        if (data instanceof Integer cardId) {
            Optional<AbstractCard> cardOpt = owner.getHand().getCards().stream()
                    .filter(c -> c.getId() == cardId)
                    .findFirst();

            if (cardOpt.isPresent()) {
                AbstractCard handCard = cardOpt.get();
                
                // Restriction: Can only swap SPADE or CLUB
                if (handCard.getSuit() != AbstractCard.Suit.SPADE && handCard.getSuit() != AbstractCard.Suit.CLUB) {
                    return null;
                }

                return List.of(new JudgementSwapperAction(owner, event, handCard));
            }
        }
        return null;
    }

    @Override
    public void onTimeout(GameManager game, Player owner, GameEvent event) {
        // Find first SPADE or CLUB
        Optional<AbstractCard> cardOpt = owner.getHand().getCards().stream()
                .filter(c -> c.getSuit() == AbstractCard.Suit.SPADE || c.getSuit() == AbstractCard.Suit.CLUB)
                .findFirst();

        if (cardOpt.isPresent()) {
            AbstractCard autoCard = cardOpt.get();
            Log.printf("JudgementSwapperSkill timed out. Auto-swapping.");
            game.getActionQueue().add(new JudgementSwapperAction(owner, event, autoCard));
        }
    }
}
