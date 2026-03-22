package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseFieldToHand;

/**
 * Adds a specific card (often a judgement card) to a player's hand and notifies the client.
 */
public class AddCardToHandAction implements GameAction {
    private final Player player;
    private final AbstractCard card;

    public AddCardToHandAction(Player player, AbstractCard card) {
        this.player = player;
        this.card = card;
    }

    @Override
    public void execute(GameManager game) {
        player.getHand().addCard(card);
        
        // Broadcast the FieldToHand response
        ResponseFieldToHand res = new ResponseFieldToHand(player.getID(), card);
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(res);
        }
    }
}
