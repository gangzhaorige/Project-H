package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponsePlayCard;
import com.zzhgl.app.utility.Log;

import java.util.ArrayList;
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
        // Anyone with this skill can trigger it when a judgement is about to resolve
        // (Alternatively, you could limit it to the person affected by the judgement)
        return event.getType() == GameEvent.EventType.BEFORE_JUDGEMENT_RESOLVE && owner.getHand().size() > 0;
    }

    @Override
    public boolean execute(GameManager game, Player owner, GameEvent event, Object data) {
        // Step 1: Player accepted the skill but hasn't picked a card yet
        if (data == null) {
            Log.printf("Player %d accepted %s. Waiting for card selection.", owner.getID(), getName());
            return false; // Not finished, wait for PlayCardCommand
        }

        // Step 2: Player picked a card (sent via SkillResponseCommand or PlayCardCommand)
        if (data instanceof Integer cardId) {
            Optional<AbstractCard> cardOpt = owner.getHand().getCards().stream()
                    .filter(c -> c.getId() == cardId)
                    .findFirst();

            if (cardOpt.isPresent()) {
                applyEffect(game, owner, event, cardOpt.get());
                return true; // Finished!
            }
        }
        return false;
    }

    private void applyEffect(GameManager game, Player owner, GameEvent event, AbstractCard newCard) {
        owner.getHand().removeCard(newCard);
        
        // Put the previous judgement card into discard pile (optional, depends on rules)
        if (event.getData() instanceof AbstractCard oldCard) {
            game.getDiscardPile().addCard(oldCard);
        }

        // Replace the event data so JudgementState picks it up
        event.setData(newCard);
        Log.printf("Player %d used %s to override judgement with %s", owner.getID(), getName(), newCard);

        // Calculate current evaluation for the UI
        boolean judgeResult = false;
        if (game.getCurrentState() instanceof com.zzhgl.app.model.States.JudgementState judgeState) {
            judgeResult = judgeState.getEffect().evaluateJudgement(game, newCard);
        }

        // Notify everyone that a card was played for judgement override
        ResponsePlayCard response = new ResponsePlayCard(owner.getID(), newCard, new ArrayList<>(), true, judgeResult);
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }
    }

    @Override
    public void onTimeout(GameManager game, Player owner, GameEvent event) {
        List<AbstractCard> cards = owner.getHand().getCards();
        if (!cards.isEmpty()) {
            AbstractCard lastCard = cards.get(cards.size() - 1);
            Log.printf("JudgementOverrideSkill timed out. Auto-using LAST card: %s", lastCard);
            applyEffect(game, owner, event, lastCard);
        } else {
            Log.printf("JudgementOverrideSkill timed out. Hand is empty, no effect applied.");
        }
    }
}
