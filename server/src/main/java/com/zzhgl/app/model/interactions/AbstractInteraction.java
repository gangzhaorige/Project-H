package com.zzhgl.app.model.interactions;

import com.zzhgl.app.model.cards.AbstractCard;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;

/**
 * AbstractInteraction represents a single atomic game action that can trigger reactions.
 * Interactions are pushed onto the GameStack and resolved sequentially.
 */
public abstract class AbstractInteraction {
    protected Player caster;
    protected Player target; // Note: Some interactions might have multiple targets or no targets, but this is a common base.
    protected AbstractCard card;
    protected boolean negatable;
    protected boolean canceled = false;

    public AbstractInteraction(Player caster, Player target, AbstractCard card, boolean negatable) {
        this.caster = caster;
        this.target = target;
        this.card = card;
        this.negatable = negatable;
    }

    public Player getCaster() { return caster; }
    public Player getTarget() { return target; }
    public AbstractCard getCard() { return card; }
    public boolean isNegatable() { return negatable; }
    public boolean isCanceled() { return canceled; }
    public void cancel() { this.canceled = true; }

    /**
     * Executes the actual logic of the interaction (e.g., subtracting HP, pushing prompt states).
     */
    public abstract void evaluate(GameManager game);
}
