package com.zzhgl.app.core;

import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.Log;

// Other Imports

public class NetworkManager {

    private NetworkManager() {
    }

    /**
     * Push a pending response to a user's queue.
     *
     * @param player_id holds the player ID
     * @param response is the instance containing the response information
     */
    public static void addResponseForUser(int player_id, GameResponse response) {
        Player player = GameServer.getInstance().getActivePlayer(player_id);

        if (player != null) {
            player.addResponseForUpdate(response);
        } else {
            Log.printf_e("Failed to create response for user, %d. Player not found.", player_id);
        }
    }

    /**
     * Push a pending response to all users' queue except one user.
     *
     * @param player_id holds the excluding player ID
     * @param response is the instance containing the response information
     */
    public static void addResponseForAllOnlinePlayers(int player_id, GameResponse response) {
        for (Player player : GameServer.getInstance().getActivePlayers()) {
            if (player != null && player.getID() != player_id) {
                player.addResponseForUpdate(response);
            }
        }
    }
}
