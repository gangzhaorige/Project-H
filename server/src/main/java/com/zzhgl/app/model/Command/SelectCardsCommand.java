package com.zzhgl.app.model.Command;

import java.util.List;

import com.zzhgl.app.model.core.GameManager;

public class SelectCardsCommand implements Command {
    private final int playerId;
    private final List<Integer> cardIndices;

    public SelectCardsCommand(int playerId, List<Integer> cardIndices) {
        this.playerId = playerId;
        this.cardIndices = cardIndices;
    }

    public int getPlayerId() { return playerId; }
    public List<Integer> getCardIndices() { return cardIndices; }

    @Override
    public void execute(GameManager game) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'execute'");
    }
}
