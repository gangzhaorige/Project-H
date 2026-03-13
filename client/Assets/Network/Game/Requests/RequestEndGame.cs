using UnityEngine;

public class RequestEndGame : NetworkRequest {
	public RequestEndGame() {
		Request_id = Constants.CMSG_END_GAME;
	}
	
	public void Send() {
	    Packet = new GamePacket(Request_id);
	}
}
