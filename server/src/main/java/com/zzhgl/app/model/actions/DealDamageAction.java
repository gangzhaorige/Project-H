package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.interactions.DamageInteraction;
import com.zzhgl.app.utility.Log;

public class DealDamageAction implements GameAction {
    private final Player source;
    private final Player target;
    private final int amount;

    public DealDamageAction(Player source, Player target, int amount) {
        this.source = source;
        this.target = target;
        this.amount = amount;
    }

    @Override
    public void execute(GameManager game) {
        Log.printf("DealDamageAction: %d damage to %d", source.getID(), target.getID());
        DamageInteraction dmg = new DamageInteraction(source, target, null, amount);
        dmg.evaluate(game);
    }
}
