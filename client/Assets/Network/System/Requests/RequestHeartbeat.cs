using UnityEngine;

using System;

public class RequestHeartBeat : NetworkRequest {

	public RequestHeartBeat() {
		Request_id = Constants.CMSG_HEARTBEAT;
	}
	
	public void Send() {
	    Packet = new GamePacket(Request_id);
	}
}