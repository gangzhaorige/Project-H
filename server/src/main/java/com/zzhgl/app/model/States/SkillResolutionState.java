package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.PlayCardCommand;
import com.zzhgl.app.model.Command.SkillResponseCommand;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.skills.AbstractSkill;
import com.zzhgl.app.networking.response.game.ResponseSkillQuery;
import com.zzhgl.app.networking.response.game.ResponseTimerCancel;
import com.zzhgl.app.networking.response.game.ResponseTimerStart;
import com.zzhgl.app.utility.Log;

import java.util.LinkedList;
import java.util.Queue;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

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
    private final ScheduledExecutorService scheduler = Executors.newSingleThreadScheduledExecutor();
    private ScheduledFuture<?> timerFuture;
    private static final int SKILL_TIMEOUT_SECONDS = 10;

    public void addSkill(Player owner, AbstractSkill skill, GameEvent event) {
        queue.add(new PendingSkill(owner, skill, event));
    }

    public boolean isEmpty() {
        return queue.isEmpty();
    }

    @Override
    public void onEnter(GameManager game) {
        resolveNext(game);
    }

    private synchronized void resolveNext(GameManager game) {
        cancelTimer(game);

        if (queue.isEmpty()) {
            game.popState();
            return;
        }

        PendingSkill next = queue.peek();
        if (next.skill.isOptional()) {
            Log.printf("Querying player %d for optional skill %s", next.owner.getID(), next.skill.getName());
            
            // Broadcast timer to everyone so they see the progress
            broadcast(game, new ResponseTimerStart(next.owner.getID(), SKILL_TIMEOUT_SECONDS, "Decision: " + next.skill.getName(), "ANY"));
            
            // Specifically notify the owner
            next.owner.addResponseForUpdate(new ResponseSkillQuery(next.owner.getID(), next.skill.getId(), next.skill.getName()));
            
            startTimer(game);
            // Wait for handleAction (SkillResponseCommand or PlayCardCommand)
        } else {
            Log.printf("Automatically executing skill %s for player %d", next.skill.getName(), next.owner.getID());
            next.skill.execute(game, next.owner, next.event, null);
            queue.poll();
            resolveNext(game);
        }
    }

    private synchronized void startTimer(GameManager game) {
        timerFuture = scheduler.schedule(() -> {
            Log.printf("Skill decision timed out for player.");
            handleTimeout(game);
        }, SKILL_TIMEOUT_SECONDS, TimeUnit.SECONDS);
    }

    private synchronized void cancelTimer(GameManager game) {
        if (timerFuture != null && !timerFuture.isDone()) {
            timerFuture.cancel(false);
            broadcast(game, new ResponseTimerCancel());
        }
    }

    private synchronized void handleTimeout(GameManager game) {
        PendingSkill current = queue.poll();
        if (current != null) {
            Log.printf("Skill %s timed out for player %d. Calling onTimeout.", current.skill.getName(), current.owner.getID());
            current.skill.onTimeout(game, current.owner, current.event);
        }
        resolveNext(game);
    }

    private void broadcast(GameManager game, com.zzhgl.app.networking.response.GameResponse response) {
        for (Player p : game.getPlayers()) {
            p.addResponseForUpdate(response);
        }
    }

    @Override
    public synchronized void handleAction(GameManager game, Command command) {
        PendingSkill current = queue.peek();
        if (current == null) return;

        if (command instanceof SkillResponseCommand respCmd) {
            if (respCmd.getPlayerId() == current.owner.getID()) {
                cancelTimer(game);
                
                if (respCmd.isAccepted()) {
                    boolean finished = current.skill.execute(game, current.owner, current.event, respCmd.getData());
                    if (finished) {
                        queue.poll(); // Remove resolved skill
                        resolveNext(game); // Move to next in queue
                    } else {
                        // Skill needs more input (e.g. a PlayCardCommand)
                        // Restart timer for the second phase of the skill
                        startTimer(game);
                        broadcast(game, new ResponseTimerStart(current.owner.getID(), SKILL_TIMEOUT_SECONDS, "Skill Input: " + current.skill.getName(), "ANY"));
                    }
                } else {
                    queue.poll(); // Refused
                    resolveNext(game);
                }
            }
        } else if (command instanceof PlayCardCommand playCmd) {
            if (playCmd.getPlayerId() == current.owner.getID()) {
                // Pass card ID as data to the skill
                boolean finished = current.skill.execute(game, current.owner, current.event, playCmd.getCardId());
                if (finished) {
                    cancelTimer(game);
                    queue.poll();
                    resolveNext(game);
                } else {
                    // Invalid input or more needed. 
                    // DO NOT cancel or restart timer. Let the original timer continue.
                    Log.printf("Skill execution for %s returned false. Invalid input? Timer continues.", current.skill.getName());
                }
            }
        }
    }

    @Override
    public void onExit(GameManager game) {
        cancelTimer(game);
        scheduler.shutdown();
    }

    @Override
    public void onPause(GameManager game) {
        cancelTimer(game);
    }

    @Override
    public void onResume(GameManager game) {
        // If we resumed (e.g. from an interaction or card play triggered by the skill),
        // we might still have skills in the queue to resolve.
        resolveNext(game);
    }
}
