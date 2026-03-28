package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.States.JudgementState;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseSwapFieldHand;
import com.zzhgl.app.utility.Log;

/**
 * Atomic action to swap a hand card with the current judgement card.
 */
public class JudgementSwapperAction implements GameAction {
    private final Player owner;
    private final GameEvent event;
    private final AbstractCard handCard;

    public JudgementSwapperAction(Player owner, GameEvent event, AbstractCard handCard) {
        this.owner = owner;
        this.event = event;
        this.handCard = handCard;
    }

    @Override
    public void execute(GameManager game) {
        if (event.getData() instanceof AbstractCard oldJudgementCard) {
            // SWAP logic
            owner.getHand().removeCard(handCard);
            owner.getHand().addCard(oldJudgementCard);
            
            // Replace judgement card in event
            event.setData(handCard);
            
            // Calculate current evaluation for the UI
            boolean judgeResult = false;
            JudgementState judgeState = game.findState(JudgementState.class);
            if (judgeState != null) {
                judgeResult = judgeState.getEffect().evaluateJudgement(game, handCard);
            }

            Log.printf("Player %d SWAPPED hand card %s with judgement card %s.", 
                       owner.getID(), handCard, oldJudgementCard);

            // Notify everyone
            ResponseSwapFieldHand response = new ResponseSwapFieldHand(owner.getID(), oldJudgementCard, handCard, judgeResult);
            for (Player p : game.getPlayers()) {
                p.addResponseForUpdate(response);
            }
        }
    }
}
