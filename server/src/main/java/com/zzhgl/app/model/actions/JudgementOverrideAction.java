package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseDiscardCard;
import com.zzhgl.app.utility.Log;

/**
 * Atomic action to override a judgement result.
 */
public class JudgementOverrideAction implements GameAction {
    private final Player owner;
    private final GameEvent event;
    private final AbstractCard newCard;

    public JudgementOverrideAction(Player owner, GameEvent event, AbstractCard newCard) {
        this.owner = owner;
        this.event = event;
        this.newCard = newCard;
    }

    @Override
    public void execute(GameManager game) {
        owner.getHand().removeCard(newCard);
        
        // Put the previous judgement card into discard pile
        if (event.getData() instanceof AbstractCard oldCard) {
            game.getDiscardPile().addCard(oldCard);
        }

        // Replace the event data so JudgementState picks it up
        event.setData(newCard);
        Log.printf("Player %d overrode judgement with %s", owner.getID(), newCard);

        // Calculate result for UI
        boolean judgeResult = false;
        if (game.getCurrentState() instanceof com.zzhgl.app.model.States.JudgementState judgeState) {
            judgeResult = judgeState.getEffect().evaluateJudgement(game, newCard);
        }

        // Broadcast to clients
        ResponseDiscardCard response = new ResponseDiscardCard(owner.getID(), newCard, true, judgeResult);
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }
    }
}
