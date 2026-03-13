using UnityEngine;

public class RequestLeaveRoom : NetworkRequest {
	public RequestLeaveRoom() {
		Request_id = Constants.CMSG_LEAVE_ROOM;
	}
	
	public void Send() {
	    Packet = new GamePacket(Request_id);
	}
}
