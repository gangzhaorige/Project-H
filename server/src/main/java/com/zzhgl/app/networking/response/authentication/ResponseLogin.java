package com.zzhgl.app.networking.response.authentication;

import com.zzhgl.app.metadata.Constants;
import com.zzhgl.app.networking.response.GameResponse;
import com.zzhgl.app.utility.GamePacket;

public class ResponseLogin extends GameResponse {

    private short status;
    private int playerId;
    private String username;
    private String roomId = "";
    private String sessionToken = "";
    private boolean inGame = false;

    public ResponseLogin() {
        responseCode = Constants.SMSG_AUTH;
    }

    @Override
    public byte[] constructResponseInBytes() {
        GamePacket packet = new GamePacket(responseCode);
        packet.addShort16(status);
        if (status == Constants.SUCCESS) {
            packet.addInt32(playerId);
            packet.addString(username);
            packet.addString(roomId);
            packet.addString(sessionToken);
            packet.addBoolean(inGame);
        }
        return packet.getBytes();
    }

    public void setSessionToken(String sessionToken) {
        this.sessionToken = sessionToken;
    }

    public void setStatus(short status) {
        this.status = status;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }

    public void setInGame(boolean inGame) {
        this.inGame = inGame;
    }
}
