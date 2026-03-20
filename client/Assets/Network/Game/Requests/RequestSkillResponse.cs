public class RequestSkillResponse : NetworkRequest {
    public RequestSkillResponse() {
        Request_id = Constants.CMSG_SKILL_RESPONSE;
    }

    public void Send(bool accepted) {
        Packet = new GamePacket(Request_id);
        Packet.AddBool(accepted);
    }
}
