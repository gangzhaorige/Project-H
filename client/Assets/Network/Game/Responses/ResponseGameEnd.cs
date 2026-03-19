using UnityEngine;

public class ResponseGameEndEventArgs : ExtendedEventArgs {
    public short Status { get; set; }

    public ResponseGameEndEventArgs() {
        Event_id = Constants.SMSG_GAME_END;
    }
}

public class ResponseGameEnd : BaseNetworkResponse {
    public ResponseGameEnd() {
        Response_id = Constants.SMSG_GAME_END;
    }
    
    protected override void ParseResponseData() {
        // For future use: WinnerId = DataReader.ReadInt(DataStream);
    }
    
    public override ExtendedEventArgs Process() {
        ResponseGameEndEventArgs args = new ResponseGameEndEventArgs();
        args.Status = status;
        
        // The game is over, we are no longer in a room.
        Constants.ROOM_ID = "";
        Constants.IS_HOST = false;
        
        return args;
    }
}
