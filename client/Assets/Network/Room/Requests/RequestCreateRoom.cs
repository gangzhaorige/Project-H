using UnityEngine;

public class RequestCreateRoom : NetworkRequest {
	public RequestCreateRoom() {
		Request_id = Constants.CMSG_CREATE_ROOM;
	}
	
	public void Send(string roomName) {
	    Packet = new GamePacket(Request_id);
        Packet.AddString(roomName);
	}
}
