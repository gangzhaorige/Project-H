using UnityEngine;

public class RequestReadyForGameSetup : NetworkRequest {
    public RequestReadyForGameSetup() {
        Request_id = Constants.CMSG_READY_FOR_GAME_SETUP;
    }

    public void Send() {
        Packet = new GamePacket(Request_id);
    }
}
