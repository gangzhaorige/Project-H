package com.zzhgl.app.networking.request.game;

import com.zzhgl.app.core.GameClient;
import com.zzhgl.app.model.Command.EndTurnCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;

import java.io.IOException;

public class RequestEndTurn extends GameRequest {
    @Override
    public void parse() throws IOException {
        // No additional data
    }

    @Override
    public void doBusiness() throws Exception {
        Room room = client.getPlayer().getCurrentRoom();
        if (room != null && room.getGameManager() != null) {
            GameManager game = room.getGameManager();
            game.handleAction(new EndTurnCommand(client.getPlayer().getID()));
        }
    }
}
