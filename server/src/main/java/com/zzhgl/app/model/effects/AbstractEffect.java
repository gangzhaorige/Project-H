package com.zzhgl.app.model.effects;

import com.zzhgl.app.model.actions.GameAction;
import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import java.util.List;

/**
 * AbstractEffect represents a lingering effect or a trigger that needs judgement.
 */
public abstract class AbstractEffect {
    protected final AbstractCard sourceCard;
    protected final Player caster;
    protected final Player target;

    public AbstractEffect(AbstractCard sourceCard, Player caster, Player target) {
        this.sourceCard = sourceCard;
        this.caster = caster;
        this.target = target;
    }

    public AbstractCard getSourceCard() { return sourceCard; }
    public Player getTarget() { return target; }
    public Player getCaster() { return caster; }

    public abstract String getName();

    /**
     * Called when the effect attempts to trigger (e.g., Judgement phase).
     * @return true if the effect successfully activates (after judgements, etc.), false otherwise.
     */
    public abstract boolean evaluateJudgement(GameManager game, AbstractCard judgementCard);

    /**
     * Executes the consequence of the effect.
     * @param game The game manager.
     * @param judgementCard The card used for judgement.
     * @return List of GameActions to be executed.
     */
    public abstract List<GameAction> applyConsequence(GameManager game, AbstractCard judgementCard);
}

