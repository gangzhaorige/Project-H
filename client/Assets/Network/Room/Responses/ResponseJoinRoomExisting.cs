using UnityEngine;

public class ResponseJoinRoomExistingEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string Message { get; set; }
    public int PlayerId { get; set; }
    public string Username { get; set; }

    public ResponseJoinRoomExistingEventArgs() {
        Event_id = Constants.SMSG_JOIN_ROOM_EXISTING;
    }
}

public class ResponseJoinRoomExisting : BaseNetworkResponse {
    private int playerId;
    private string username;

    public ResponseJoinRoomExisting() {
        Response_id = Constants.SMSG_JOIN_ROOM_EXISTING;
    }
    
    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        username = DataReader.ReadString(DataStream);
    }
    
    public override ExtendedEventArgs Process() {
        ResponseJoinRoomExistingEventArgs args = new ResponseJoinRoomExistingEventArgs();
        args.Status = status;
        args.Message = message;
        args.PlayerId = playerId;
        args.Username = username;
        return args;
    }
}
