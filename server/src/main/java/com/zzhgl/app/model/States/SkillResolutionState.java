package com.zzhgl.app.model.States;

import com.zzhgl.app.model.Command.Command;
import com.zzhgl.app.model.Command.PlayCardCommand;
import com.zzhgl.app.model.Command.SkillResponseCommand;
import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.core.GameEvent;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.skills.AbstractSkill;
import com.zzhgl.app.networking.response.game.ResponseSkillActivated;
import com.zzhgl.app.networking.response.game.ResponseSkillQuery;
import com.zzhgl.app.networking.response.game.ResponseTimerCancel;
import com.zzhgl.app.networking.response.game.ResponseTimerStart;
import com.zzhgl.app.utility.Log;

import java.util.LinkedList;
import java.util.Queue;
import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

/**
 * SkillResolutionState manages a queue of skills that were triggered by an event.
 * It resolves them one by one. If a skill produces actions, it pushes ActionResolveState.
 */
public class SkillResolutionState implements GameState {
    
    private static class PendingSkill {
        Player owner;
        AbstractSkill skill;
        GameEvent event;
        boolean isWaitingForInput = false;

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

        // 1. If there are existing actions to process, push ActionResolveState
        game.resolveActions();
        if (game.getCurrentState() != this) return;

        // 2. If no more skills to resolve, finish
        if (queue.isEmpty()) {
            if (game.getCurrentState() == this) {
                game.popState();
            }
            return;
        }

        PendingSkill next = queue.peek();
        
        // 3. If waiting for card selection/input, don't re-query
        if (next.isWaitingForInput) {
            startTimer(game);
            return;
        }

        // 4. Handle Skill Trigger
        if (next.skill.isOptional()) {
            Log.printf("Querying player %d for optional skill %s", next.owner.getID(), next.skill.getName());
            broadcast(game, new ResponseTimerStart(next.owner.getID(), SKILL_TIMEOUT_SECONDS, "Decision: " + next.skill.getName(), "ANY"));
            next.owner.addResponseForUpdate(new ResponseSkillQuery(next.owner.getID(), next.skill.getId(), next.skill.getName()));
            startTimer(game);
        } else {
            Log.printf("Automatically executing skill %s for player %d", next.skill.getName(), next.owner.getID());
            List<GameAction> actions = next.skill.execute(game, next.owner, next.event, null);
            
            queue.poll(); // Skill evaluated

            if (actions != null && !actions.isEmpty()) {
                game.getActionQueue().addAll(actions);
                notifySkillActivation(game, next.owner, next.skill);
                game.resolveActions();
            } else {
                resolveNext(game);
            }
        }
    }

    private void notifySkillActivation(GameManager game, Player owner, AbstractSkill skill) {
        if (owner.getSelectedChampion() != null) {
            java.util.List<AbstractSkill> skills = owner.getSelectedChampion().getSkills();
            int index = skills.indexOf(skill);
            if (index != -1) {
                broadcast(game, new ResponseSkillActivated(owner.getID(), index));
            }
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

        if (command instanceof SkillResponseCommand respCmd && respCmd.getPlayerId() == current.owner.getID()) {
            cancelTimer(game);
            if (respCmd.isAccepted()) {
                current.isWaitingForInput = false;
                List<GameAction> actions = current.skill.execute(game, current.owner, current.event, respCmd.getSkillId());
                if (actions != null && !actions.isEmpty()) {
                    game.getActionQueue().addAll(actions);
                    notifySkillActivation(game, current.owner, current.skill);
                    queue.poll();
                    game.resolveActions();
                } else {
                    // Skill returned null/empty but was accepted - likely needs more input (PlayCard)
                    current.isWaitingForInput = true;
                    startTimer(game);
                    broadcast(game, new ResponseTimerStart(current.owner.getID(), SKILL_TIMEOUT_SECONDS, "Skill Input: " + current.skill.getName(), "ANY"));
                }
            } else {
                queue.poll();
                resolveNext(game);
            }
        } else if (command instanceof PlayCardCommand playCmd && playCmd.getPlayerId() == current.owner.getID()) {
            current.isWaitingForInput = false;
            List<GameAction> actions = current.skill.execute(game, current.owner, current.event, playCmd.getCardId());
            if (actions != null && !actions.isEmpty()) {
                game.getActionQueue().addAll(actions);
                notifySkillActivation(game, current.owner, current.skill);
                cancelTimer(game);
                queue.poll();
                game.resolveActions();
            } else {
                current.isWaitingForInput = true;
                Log.printf("Skill execution for %s returned null/empty. Invalid input? Timer continues.", current.skill.getName());
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
        // Continue with next skill or finish
        resolveNext(game);
    }
}
