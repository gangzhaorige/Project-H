using UnityEngine;
using System.Collections.Generic;

public class ResponseNotifyForChampionPickEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string Message { get; set; }
    public int ActivePlayerId { get; set; }
    public int Timeout { get; set; }

    public ResponseNotifyForChampionPickEventArgs() {
        Event_id = Constants.SMSG_NOTIFY_FOR_CHAMPION_PICK;
    }
}

public class ResponseNotifyForChampionPick : BaseNetworkResponse {
    private int activePlayerId;
    private int timeout;

    public ResponseNotifyForChampionPick() {
        Response_id = Constants.SMSG_NOTIFY_FOR_CHAMPION_PICK;
    }
    
    protected override void ParseResponseData() {
        activePlayerId = DataReader.ReadInt(DataStream);
        timeout = DataReader.ReadInt(DataStream);
    }
    
    public override ExtendedEventArgs Process() {
        ResponseNotifyForChampionPickEventArgs args = new ResponseNotifyForChampionPickEventArgs();
        args.Status = status;
        args.Message = message;
        args.ActivePlayerId = activePlayerId;
        args.Timeout = timeout;
        return args;
    }
}
