public class ResponseGameStateEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string StateName { get; set; }

    public ResponseGameStateEventArgs() {
        Event_id = Constants.SMSG_STATE_CHANGE;
    }
}

public class ResponseGameState : BaseNetworkResponse {
    private string stateName;

    public ResponseGameState() {
        Response_id = Constants.SMSG_STATE_CHANGE;
    }

    protected override void ParseResponseData() {
        stateName = DataReader.ReadString(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseGameStateEventArgs args = new ResponseGameStateEventArgs();
        args.Status = status;
        args.StateName = stateName;
        return args;
    }
}