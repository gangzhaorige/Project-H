using UnityEngine;

public class ResponseLeaveRoomEventArgs : ExtendedEventArgs {
    public short Status { get; set; }

    public ResponseLeaveRoomEventArgs() {
        Event_id = Constants.SMSG_LEAVE_ROOM;
    }
}

public class ResponseLeaveRoom : BaseNetworkResponse {
    public ResponseLeaveRoom() {
        Response_id = Constants.SMSG_LEAVE_ROOM;
    }
    
    protected override void ParseResponseData() {
        // No extra data
    }
    
    public override ExtendedEventArgs Process() {
        ResponseLeaveRoomEventArgs args = new ResponseLeaveRoomEventArgs();
        args.Status = status;
        
        if (status == Constants.SUCCESS) {
            Constants.ROOM_ID = "";
            Constants.IS_HOST = false;
        }
        
        return args;
    }
}
