package com.zzhgl.app.networking.request.game;

import com.zzhgl.app.model.Command.SelectCardsCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.utility.DataReader;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class RequestSelectCards extends GameRequest {
    private List<Integer> cardIndices;

    @Override
    public void parse() throws IOException {
        short count = DataReader.readShort(dataInput);
        cardIndices = new ArrayList<>();
        for (int i = 0; i < count; i++) {
            cardIndices.add(DataReader.readInt(dataInput));
        }
    }

    @Override
    public void doBusiness() throws Exception {
        Room room = client.getPlayer().getCurrentRoom();
        if (room != null && room.getGameManager() != null) {
            GameManager game = room.getGameManager();
            game.handleAction(new SelectCardsCommand(client.getPlayer().getID(), cardIndices));
        }
    }
}
