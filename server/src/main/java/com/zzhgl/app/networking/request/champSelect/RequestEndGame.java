package com.zzhgl.app.networking.request.champSelect;

import java.io.IOException;

import com.zzhgl.app.core.RoomManager;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.utility.Log;

public class RequestEndGame extends GameRequest {

    @Override
    public void parse() throws IOException {
    }

    @Override
    public void doBusiness() throws Exception {
        Player player = client.getPlayer();
        if (player == null) return;

        Room room = player.getCurrentRoom();
        if (room != null && room.getHostPlayerId() == player.getID() && room.isInGame()) {
            Log.printf("Player %s requested to end the game in room %s", player.getUsername(), room.getName());
            RoomManager.getInstance().endGame(room.getId());
        }
    }
}
