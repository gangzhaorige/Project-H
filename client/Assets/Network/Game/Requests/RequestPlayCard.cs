using UnityEngine;
using System.Collections.Generic;

public class RequestPlayCard : NetworkRequest {
    public RequestPlayCard() {
        Request_id = Constants.CMSG_PLAY_CARD;
    }

    public void Send(int cardId, List<int> targetIds) {
        Packet = new GamePacket(Request_id);
        Packet.AddInt32(cardId);
        
        // Add number of targets
        Packet.AddShort16((short)targetIds.Count);
        foreach(int id in targetIds) {
            Packet.AddInt32(id);
        }
    }
}
