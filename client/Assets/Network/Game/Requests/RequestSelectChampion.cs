using UnityEngine;

public class RequestSelectChampion : NetworkRequest {
	public RequestSelectChampion() {
		Request_id = Constants.CMSG_SELECT_CHAMPION;
	}
	
	public void Send(int championId) {
	    Packet = new GamePacket(Request_id);
        Packet.AddInt32(championId);
	}
}
