package com.zzhgl.app.model.skills;

import com.zzhgl.app.model.States.JudgementState;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.actions.PushStateAction;
import com.zzhgl.app.model.actions.SetFlagAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.effects.MyTurnEffect;
import com.zzhgl.app.utility.Log;

import java.util.List;

/**
 * Skill: My Turn (Seele). Optional on TURN_BEGIN.
 * If accepted:
 * - Skip draw phase.
 * - Enter JudgementState loop for Spade/Club cards.
 */
public class MyTurnSkill extends AbstractSkill {

    public MyTurnSkill() {
        super(6, "My Turn", true); // ID 6, Optional
    }

    @Override
    public GameEvent.EventType getSubscribedEvent() {
        return GameEvent.EventType.TURN_BEGIN;
    }

    @Override
    public boolean canTrigger(GameManager game, GameEvent event, Player owner) {
        // Trigger only on owner's turn begin
        int activePlayerIndex = game.getActivePlayerIndex();
        Player activePlayer = game.getPlayers().get(activePlayerIndex);
        return owner.equals(activePlayer);
    }

    @Override
    public List<GameAction> execute(GameManager game, Player owner, GameEvent event, Object data) {
        Log.printf("Seele (Player %d) uses skill: %s!", owner.getID(), getName());
        
        return List.of(
            new SetFlagAction("skipDrawPhase", GameManager::setSkipDrawPhase, true),
            new PushStateAction(new JudgementState(new MyTurnEffect(owner)))
        );
    }
}
