public class ResponseGameStartEventArgs : ExtendedEventArgs {
    public short Status { get; set; }

    public ResponseGameStartEventArgs() {
        Event_id = Constants.SMSG_GAME_START;
    }
}

public class ResponseGameStart : BaseNetworkResponse {
    public ResponseGameStart() {
        Response_id = Constants.SMSG_GAME_START;
    }
    
    protected override void ParseResponseData() {
        // No extra payload, just success and message.
    }
    
    public override ExtendedEventArgs Process() {
        ResponseGameStartEventArgs args = new ResponseGameStartEventArgs();
        args.Status = status;
        return args;
    }
}
