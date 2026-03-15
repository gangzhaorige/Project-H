package com.zzhgl.app.model.interactions;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;

/**
 * AbstractInteraction represents a single atomic game action that can trigger reactions.
 */
public abstract class AbstractInteraction {
    protected Player source;
    protected Player target;

    public AbstractInteraction(Player source, Player target) {
        this.source = source;
        this.target = target;
    }

    public Player getSource() { return source; }
    public Player getTarget() { return target; }

    /**
     * Executes the actual logic of the interaction (e.g., subtracting HP).
     */
    public abstract void execute(GameManager game);
}
