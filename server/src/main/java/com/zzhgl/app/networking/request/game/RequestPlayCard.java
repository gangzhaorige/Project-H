package com.zzhgl.app.networking.request.game;

import com.zzhgl.app.model.Command.PlayCardCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.utility.DataReader;
import com.zzhgl.app.utility.Log;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class RequestPlayCard extends GameRequest {
    private int cardId;
    private List<Integer> targetIds;

    @Override
    public void parse() throws IOException {
        cardId = DataReader.readInt(dataInput);
        short targetCount = DataReader.readShort(dataInput);
        
        targetIds = new ArrayList<>();
        for (int i = 0; i < targetCount; i++) {
            targetIds.add(DataReader.readInt(dataInput));
        }
    }

    @Override
    public void doBusiness() throws Exception {
        Room room = client.getPlayer().getCurrentRoom();
        if (room != null && room.getGameManager() != null) {
            GameManager game = room.getGameManager();
            game.handleAction(new PlayCardCommand(client.getPlayer().getID(), cardId, targetIds));
        }
    }
}
