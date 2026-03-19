using UnityEngine;

public class ResponseMatchStateEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int ConnectedPlayers { get; set; }
    public int TotalPlayers { get; set; }

    public ResponseMatchStateEventArgs() {
        Event_id = Constants.SMSG_MATCH_STATE;
    }
}

public class ResponseMatchState : BaseNetworkResponse {
    private int connectedPlayers;
    private int totalPlayers;

    public ResponseMatchState() {
        Response_id = Constants.SMSG_MATCH_STATE;
    }
    
    protected override void ParseResponseData() {
        connectedPlayers = DataReader.ReadInt(DataStream);
        totalPlayers = DataReader.ReadInt(DataStream);
    }
    
    public override ExtendedEventArgs Process() {
        ResponseMatchStateEventArgs args = new ResponseMatchStateEventArgs();
        args.Status = status;
        args.ConnectedPlayers = connectedPlayers;
        args.TotalPlayers = totalPlayers;
        return args;
    }
}
