using UnityEngine;

public class RequestStartGame : NetworkRequest {
	public RequestStartGame() {
		Request_id = Constants.CMSG_GAME_START;
	}
	
	public void Send() {
	    Packet = new GamePacket(Request_id);
	}
}
