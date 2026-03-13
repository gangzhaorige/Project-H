using UnityEngine;
using System;
using System.Collections.Generic;

public class ResponseHeartBeatEventArgs : ExtendedEventArgs {
    
    public ResponseHeartBeatEventArgs() {
        Event_id = Constants.SMSG_HEARTBEAT;
    }
}

public class ResponseHeartBeat : BaseNetworkResponse {


    public ResponseHeartBeat()
    {
        Response_id = Constants.SMSG_HEARTBEAT;
    }
    
    protected override void ParseResponseData() {
    }
    
    public override ExtendedEventArgs Process() {
        ResponseHeartBeatEventArgs args = new ResponseHeartBeatEventArgs();
        SetBaseEventArgs(args);
        return args;
    }
}