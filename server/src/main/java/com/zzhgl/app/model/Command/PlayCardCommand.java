package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;
import java.util.List;

public class PlayCardCommand implements Command {
    private int playerId;
    private int cardId;
    private List<Integer> targetIds;

    public PlayCardCommand(int playerId, int cardId, List<Integer> targetIds) {
        this.playerId = playerId;
        this.cardId = cardId;
        this.targetIds = targetIds;
    }

    public int getPlayerId() { return playerId; }
    public int getCardId() { return cardId; }
    public List<Integer> getTargetIds() { return targetIds; }

    @Override
    public void execute(GameManager game) {
        // Implementation logic will be handled by the state that receives this command
    }
}
