package com.zzhgl.app.networking.request.game;

import com.zzhgl.app.model.Command.SkillResponseCommand;
import com.zzhgl.app.model.core.GameManager;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.utility.DataReader;

import java.io.IOException;

public class RequestSkillResponse extends GameRequest {
    private boolean accepted;
    private int skillId;
    @Override
    public void parse() throws IOException {
        skillId = DataReader.readInt(dataInput);
        accepted = DataReader.readBoolean(dataInput);
    }

    @Override
    public void doBusiness() throws Exception {
        Room room = client.getPlayer().getCurrentRoom();
        if (room != null && room.getGameManager() != null) {
            GameManager game = room.getGameManager();
            game.handleAction(new SkillResponseCommand(client.getPlayer().getID(), accepted, skillId));
        }
    }
}
