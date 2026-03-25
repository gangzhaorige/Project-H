using UnityEngine;
using System.Collections.Generic;

public class RequestSelectCards : NetworkRequest {
    public RequestSelectCards() {
        Request_id = Constants.CMSG_SELECT_CARDS;
    }

    public void Send(List<int> cardIndices) {
        Debug.Log($"[RequestSelectCards] Sending: ID={Request_id}, indicesCount={cardIndices.Count}, indices=[{string.Join(", ", cardIndices)}]");
        Packet = new GamePacket(Request_id);
        
        // Add number of indices
        Packet.AddShort16((short)cardIndices.Count);
        foreach(int index in cardIndices) {
            Packet.AddInt32(index);
        }
    }
}
