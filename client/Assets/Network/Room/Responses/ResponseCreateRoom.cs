using UnityEngine;

public class ResponseCreateRoomEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string RoomId { get; set; }
    public string RoomName { get; set; }

    public ResponseCreateRoomEventArgs() {
        Event_id = Constants.SMSG_CREATE_ROOM;
    }
}

public class ResponseCreateRoom : BaseNetworkResponse {
    private string roomId = "";
    private string roomName = "";

    public ResponseCreateRoom() {
        Response_id = Constants.SMSG_CREATE_ROOM;
    }
    
    protected override void ParseResponseData() {
        if (status == Constants.SUCCESS) {
            roomId = DataReader.ReadString(DataStream);
            roomName = DataReader.ReadString(DataStream);
        }
    }
    
    public override ExtendedEventArgs Process() {
        ResponseCreateRoomEventArgs args = new ResponseCreateRoomEventArgs();
        args.Status = status;
        args.RoomId = roomId;
        args.RoomName = roomName;
        
        if (status == Constants.SUCCESS) {
            Constants.ROOM_ID = roomId;
            Constants.IS_HOST = true;
        }
        
        return args;
    }
}
