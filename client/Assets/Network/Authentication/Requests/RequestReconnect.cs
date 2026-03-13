using UnityEngine;

public class RequestReconnect : NetworkRequest {
	public RequestReconnect() {
		Request_id = Constants.CMSG_RECONNECT;
	}
	
	public void Send(string username, string sessionToken) {
	    Packet = new GamePacket(Request_id);
        Packet.AddString(username);
        Packet.AddString(sessionToken);
	}
}
