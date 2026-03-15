package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;

public class SkillResponseCommand implements Command {
    private int playerId;
    private boolean accepted;
    private Object data;

    public SkillResponseCommand(int playerId, boolean accepted, Object data) {
        this.playerId = playerId;
        this.accepted = accepted;
        this.data = data;
    }

    public int getPlayerId() { return playerId; }
    public boolean isAccepted() { return accepted; }
    public Object getData() { return data; }

    @Override
    public void execute(GameManager game) {
        game.handleAction(this);
    }
}
