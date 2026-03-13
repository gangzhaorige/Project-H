using UnityEngine;

public class RequestAllRooms : NetworkRequest {
	public RequestAllRooms() {
		Request_id = Constants.CMSG_ALL_ROOMS;
	}
	
	public void Send() {
	    Packet = new GamePacket(Request_id);
	}
}
