using UnityEngine;

public class ResponseReconnectEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string RoomId { get; set; }
    // public GameStateData StateSync { get; set; } // Placeholder

    public ResponseReconnectEventArgs() {
        Event_id = Constants.SMSG_RECONNECT;
    }
}

public class ResponseReconnect : BaseNetworkResponse {
    private string roomId;

    public ResponseReconnect() {
        Response_id = Constants.SMSG_RECONNECT;
    }
    
    protected override void ParseResponseData() {
        roomId = DataReader.ReadString(DataStream);
        
        // TODO: Parse the massive game state chunk here
        // byte[] stateData = DataReader.ReadBytes(DataStream...);
    }
    
    public override ExtendedEventArgs Process() {
        ResponseReconnectEventArgs args = new ResponseReconnectEventArgs();
        args.Status = status;
        args.RoomId = roomId;
        
        if (status == Constants.SUCCESS) {
            Constants.ROOM_ID = roomId;
        }
        
        return args;
    }
}
