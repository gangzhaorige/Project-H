using UnityEngine;

public class ResponseEndTurnEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int EndedPlayerId { get; set; }

    public ResponseEndTurnEventArgs() {
        Event_id = Constants.SMSG_END_TURN;
    }
}

public class ResponseEndTurn : BaseNetworkResponse {
    private int endedPlayerId;

    public ResponseEndTurn() {
        Response_id = Constants.SMSG_END_TURN;
    }

    protected override void ParseResponseData() {
        endedPlayerId = DataReader.ReadInt(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseEndTurnEventArgs args = new ResponseEndTurnEventArgs();
        args.Status = status;
        args.EndedPlayerId = endedPlayerId;
        return args;
    }
}
