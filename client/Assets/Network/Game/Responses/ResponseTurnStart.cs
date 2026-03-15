using UnityEngine;

public class ResponseTurnStartEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int ActivePlayerId { get; set; }

    public ResponseTurnStartEventArgs() {
        Event_id = Constants.SMSG_TURN_START;
    }
}

public class ResponseTurnStart : BaseNetworkResponse {
    private int activePlayerId;

    public ResponseTurnStart() {
        Response_id = Constants.SMSG_TURN_START;
    }

    protected override void ParseResponseData() {
        activePlayerId = DataReader.ReadInt(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseTurnStartEventArgs args = new ResponseTurnStartEventArgs();
        args.Status = status;
        args.ActivePlayerId = activePlayerId;
        return args;
    }
}
