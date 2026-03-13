using UnityEngine;

public class ResponseJoinRoomEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string Message { get; set; }
    public string RoomId { get; set; }
    public string RoomName { get; set; }

    public ResponseJoinRoomEventArgs() {
        Event_id = Constants.SMSG_JOIN_ROOM;
    }
}

public class ResponseJoinRoom : BaseNetworkResponse {
    private string roomId = "";
    private string roomName = "";

    public ResponseJoinRoom() {
        Response_id = Constants.SMSG_JOIN_ROOM;
    }
    
    protected override void ParseResponseData() {
        if (status == Constants.SUCCESS) {
            roomId = DataReader.ReadString(DataStream);
            roomName = DataReader.ReadString(DataStream);
        }
    }
    
    public override ExtendedEventArgs Process() {
        ResponseJoinRoomEventArgs args = new ResponseJoinRoomEventArgs();
        args.Status = status;
        args.Message = message;
        args.RoomId = roomId;
        args.RoomName = roomName;
        
        if (status == Constants.SUCCESS) {
            Constants.ROOM_ID = roomId;
            Constants.IS_HOST = false;
        }
        
        return args;
    }
}
