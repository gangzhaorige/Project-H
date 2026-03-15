using UnityEngine;

public class RequestEndTurn : NetworkRequest {
    public RequestEndTurn() {
        Request_id = Constants.CMSG_END_TURN;
    }

    public void Send() {
        Packet = new GamePacket(Request_id);
    }
}
