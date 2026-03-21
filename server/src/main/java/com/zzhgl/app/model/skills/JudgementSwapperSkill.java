package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseSwapFieldHand;
import com.zzhgl.app.utility.Log;

import java.util.ArrayList;
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
    public boolean execute(GameManager game, Player owner, GameEvent event, Object data) {
        // Step 1: Player accepted the skill
        if (data == null) {
            Log.printf("Player %d accepted %s. Waiting for card selection.", owner.getID(), getName());
            return false; // Wait for card input
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
                    Log.printf_e("Player %d tried to swap with invalid suit: %s", owner.getID(), handCard.getSuit());
                    return false;
                }

                applyEffect(game, owner, event, handCard);
                return true;
            }
        }
        return false;
    }

    private void applyEffect(GameManager game, Player owner, GameEvent event, AbstractCard handCard) {
        if (event.getData() instanceof AbstractCard oldJudgementCard) {
            // SWAP: remove chosen card from hand, add old judgement card to hand
            owner.getHand().removeCard(handCard);
            owner.getHand().addCard(oldJudgementCard);
            
            // Replace the event data so JudgementState picks it up as the new result
            event.setData(handCard);
            
            // Calculate current evaluation for the UI
            boolean judgeResult = false;
            if (game.getCurrentState() instanceof com.zzhgl.app.model.States.JudgementState judgeState) {
                judgeResult = judgeState.getEffect().evaluateJudgement(game, handCard);
            }

            Log.printf("Player %d used %s to SWAP hand card %s with judgement card %s. New result: %b", 
                       owner.getID(), getName(), handCard, oldJudgementCard, judgeResult);

            // Notify everyone about the SWAP
            ResponseSwapFieldHand response = new ResponseSwapFieldHand(owner.getID(), oldJudgementCard, handCard, judgeResult);
            for (Player p : game.getPlayers()) {
                p.addResponseForUpdate(response);
            }
        }
    }

    @Override
    public void onTimeout(GameManager game, Player owner, GameEvent event) {
        // Find first SPADE or CLUB
        Optional<AbstractCard> cardOpt = owner.getHand().getCards().stream()
                .filter(c -> c.getSuit() == AbstractCard.Suit.SPADE || c.getSuit() == AbstractCard.Suit.CLUB)
                .findFirst();

        if (cardOpt.isPresent()) {
            AbstractCard autoCard = cardOpt.get();
            Log.printf("JudgementSwapperSkill timed out. Auto-swapping with: %s", autoCard);
            applyEffect(game, owner, event, autoCard);
        } else {
            Log.printf("JudgementSwapperSkill timed out. No valid card to swap.");
        }
    }
}
