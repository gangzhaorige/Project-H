using UnityEngine;

public class RequestPickChampion : NetworkRequest {
	public RequestPickChampion() {
		Request_id = Constants.CMSG_PICK_CHAMPION;
	}
	
	public void Send(int championId) {
	    Packet = new GamePacket(Request_id);
        Packet.AddInt32(championId);
	}
}
