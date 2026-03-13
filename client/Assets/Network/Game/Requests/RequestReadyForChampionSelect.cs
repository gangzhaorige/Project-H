using UnityEngine;

public class RequestReadyForChampionSelect : NetworkRequest {
	public RequestReadyForChampionSelect() {
		Request_id = Constants.CMSG_READY_FOR_CHAMPION_SELECT;
	}
	
	public void Send() {
	    Packet = new GamePacket(Request_id);
	}
}
