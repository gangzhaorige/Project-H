package com.zzhgl.app.model.effects;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;

/**
 * AbstractEffect represents a continuous status attached to a player.
 */
public abstract class AbstractEffect {
    protected AbstractCard sourceCard;
    protected Player target;
    protected Player caster;

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
     */
    public abstract void applyConsequence(GameManager game);
}
