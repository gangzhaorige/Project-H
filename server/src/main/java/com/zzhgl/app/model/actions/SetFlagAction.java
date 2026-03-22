package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.utility.Log;

import java.util.function.BiConsumer;

/**
 * SetFlagAction updates a specific flag (like skipDrawPhase) on the GameManager.
 */
public class SetFlagAction implements GameAction {
    private final String flagName;
    private final BiConsumer<GameManager, Boolean> flagSetter;
    private final boolean value;

    public SetFlagAction(String flagName, BiConsumer<GameManager, Boolean> flagSetter, boolean value) {
        this.flagName = flagName;
        this.flagSetter = flagSetter;
        this.value = value;
    }

    @Override
    public void execute(GameManager game) {
        Log.printf("SetFlagAction: setting %s to %b", flagName, value);
        flagSetter.accept(game, value);
    }
}
