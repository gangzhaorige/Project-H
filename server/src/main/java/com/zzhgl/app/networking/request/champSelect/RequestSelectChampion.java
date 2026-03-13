package com.zzhgl.app.networking.request.champSelect;

import com.zzhgl.app.model.Command.SelectChampionHoverCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.utility.DataReader;

import java.io.IOException;

public class RequestSelectChampion extends GameRequest {
    private int championId;

    @Override
    public void parse() throws IOException {
        championId = DataReader.readInt(dataInput);
    }

    @Override
    public void doBusiness() throws Exception {
        Room room = client.getPlayer().getCurrentRoom();
        if (room != null && room.isInGame()) {
            GameManager game = room.getGameManager();
            if (game != null) {
                game.handleAction(new SelectChampionHoverCommand(client.getPlayer().getID(), championId));
            }
        }
    }
}
