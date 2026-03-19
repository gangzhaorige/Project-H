using UnityEngine;

public class RequestPassPriority : NetworkRequest {
    public RequestPassPriority() {
        Request_id = Constants.CMSG_PASS_PRIORITY;
    }

    public void Send() {
        Packet = new GamePacket(Request_id);
    }
}
