package com.zzhgl.app.networking.request.champSelect;

import java.io.IOException;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.game.ResponseGameStart;
import com.zzhgl.app.utility.Log;

public class RequestStartGame extends GameRequest {

    @Override
    public void parse() throws IOException {
    }

    @Override
    public void doBusiness() throws Exception {
        Player player = client.getPlayer();
        if (player == null) return;

        Room room = player.getCurrentRoom();
        if (room != null && room.getHostPlayerId() == player.getID() && !room.isInGame()) {
            room.startGame(); // Starts inGame and the periodic broadcast
            Log.printf("Game started in room %s", room.getName());
        }
    }
}
