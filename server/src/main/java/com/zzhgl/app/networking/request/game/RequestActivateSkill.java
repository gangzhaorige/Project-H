package com.zzhgl.app.networking.request.game;

import com.zzhgl.app.model.Command.ActivateSkillCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.utility.DataReader;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class RequestActivateSkill extends GameRequest {
    private int skillId;
    private List<Integer> discardCardIds;
    private List<Integer> targetIds;

    @Override
    public void parse() throws IOException {
        skillId = DataReader.readInt(dataInput);
        
        short discardCount = DataReader.readShort(dataInput);
        discardCardIds = new ArrayList<>();
        for (int i = 0; i < discardCount; i++) {
            discardCardIds.add(DataReader.readInt(dataInput));
        }

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
            game.handleAction(new ActivateSkillCommand(client.getPlayer().getID(), skillId, discardCardIds, targetIds));
        }
    }
}
