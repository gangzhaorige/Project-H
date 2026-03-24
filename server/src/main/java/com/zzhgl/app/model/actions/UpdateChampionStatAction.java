package com.zzhgl.app.model.actions;

import com.zzhgl.app.model.champions.Champion;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.game.ResponseChampionStatsUpdateInteger;
import com.zzhgl.app.utility.Log;

import java.util.function.BiConsumer;

/**
 * Action to update a champion's stat and notify clients.
 */
public class UpdateChampionStatAction implements GameAction {
    private final Player player;
    private final int statId;
    private final int value;
    private final BiConsumer<Champion, Integer> setter;

    public UpdateChampionStatAction(Player player, int statId, int value, BiConsumer<Champion, Integer> setter) {
        this.player = player;
        this.statId = statId;
        this.value = value;
        this.setter = setter;
    }

    @Override
    public void execute(GameManager game) {
        Champion champ = player.getSelectedChampion();
        if (champ != null) {
            Log.printf("UpdateChampionStatAction: Setting Stat %d to %d for Player %d", statId, value, player.getID());
            setter.accept(champ, value);
            
            // Notify clients
            ResponseChampionStatsUpdateInteger res = new ResponseChampionStatsUpdateInteger(champ.getId(), statId, value);
            for (Player p : game.getPlayers()) {
                p.addResponseForUpdate(res);
            }
        }
    }
}
