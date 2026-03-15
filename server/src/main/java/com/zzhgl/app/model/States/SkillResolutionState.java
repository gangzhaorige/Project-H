package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.SkillResponseCommand;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.skills.AbstractSkill;
import com.zzhgl.app.networking.response.game.ResponseSkillQuery;
import com.zzhgl.app.utility.Log;

import java.util.LinkedList;
import java.util.Queue;

/**
 * SkillResolutionState manages a queue of skills that were triggered by an event.
 * It resolves them one by one in order.
 */
public class SkillResolutionState implements GameState {
    
    private static class PendingSkill {
        Player owner;
        AbstractSkill skill;
        GameEvent event;

        PendingSkill(Player owner, AbstractSkill skill, GameEvent event) {
            this.owner = owner;
            this.skill = skill;
            this.event = event;
        }
    }

    private final Queue<PendingSkill> queue = new LinkedList<>();

    public void addSkill(Player owner, AbstractSkill skill, GameEvent event) {
        queue.add(new PendingSkill(owner, skill, event));
    }

    @Override
    public void onEnter(GameManager game) {
        resolveNext(game);
    }

    private void resolveNext(GameManager game) {
        if (queue.isEmpty()) {
            game.popState();
            return;
        }

        PendingSkill next = queue.peek();
        if (next.skill.isOptional()) {
            Log.printf("Querying player %d for optional skill %s", next.owner.getID(), next.skill.getName());
            next.owner.addResponseForUpdate(new ResponseSkillQuery(next.skill.getId(), next.skill.getName()));
            // Wait for handleAction (SkillResponseCommand)
        } else {
            Log.printf("Automatically executing skill %s for player %d", next.skill.getName(), next.owner.getID());
            next.skill.execute(game, next.owner, next.event, null);
            queue.poll();
            resolveNext(game);
        }
    }

    @Override
    public synchronized void handleAction(GameManager game, Command command) {
        if (command instanceof SkillResponseCommand respCmd) {
            PendingSkill current = queue.peek();
            if (current != null && respCmd.getPlayerId() == current.owner.getID()) {
                if (respCmd.isAccepted()) {
                    current.skill.execute(game, current.owner, current.event, respCmd.getData());
                }
                
                queue.poll(); // Remove resolved skill
                resolveNext(game); // Move to next in queue
            }
        }
    }

    @Override
    public void onExit(GameManager game) {}

    @Override
    public void onPause(GameManager game) {}

    @Override
    public void onResume(GameManager game) {}
}
