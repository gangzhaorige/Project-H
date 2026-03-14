using UnityEngine;

public class RequestReadyToPlay : NetworkRequest {
    public RequestReadyToPlay() {
        Request_id = Constants.CMSG_READY_TO_PLAY;
    }

    public void Send() {
        Packet = new GamePacket(Request_id);
    }
}
