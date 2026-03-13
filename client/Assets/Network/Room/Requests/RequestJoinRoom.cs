using UnityEngine;

public class RequestJoinRoom : NetworkRequest {
	public RequestJoinRoom() {
		Request_id = Constants.CMSG_JOIN_ROOM;
	}
	
	public void Send(string roomId) {
	    Packet = new GamePacket(Request_id);
        Packet.AddString(roomId);
	}
}
