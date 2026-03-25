using UnityEngine;
using System.Collections.Generic;

public class RequestActivateSkill : NetworkRequest {
    public RequestActivateSkill() {
        Request_id = Constants.CMSG_ACTIVATE_SKILL;
    }

    public void Send(int skillId, List<int> discardCardIds, List<int> targetIds) {
        Debug.Log($"[RequestActivateSkill] Sending: ID={Request_id}, skillId={skillId}, discardsCount={discardCardIds.Count}, targetsCount={targetIds.Count}");
        Packet = new GamePacket(Request_id);
        Packet.AddInt32(skillId);
        
        Packet.AddShort16((short)discardCardIds.Count);
        foreach(int id in discardCardIds) {
            Packet.AddInt32(id);
        }

        Packet.AddShort16((short)targetIds.Count);
        foreach(int id in targetIds) {
            Packet.AddInt32(id);
        }
    }
}
