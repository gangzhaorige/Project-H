public class RequestSkillResponse : NetworkRequest {
    public RequestSkillResponse() {
        Request_id = Constants.CMSG_SKILL_RESPONSE;
    }

    public void Send(int skillId, bool accepted) {
        Packet = new GamePacket(Request_id);
        Packet.AddInt32(skillId);
        Packet.AddBool(accepted);
    }
}
