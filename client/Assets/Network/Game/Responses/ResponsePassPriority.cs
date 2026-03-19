using UnityEngine;

public class ResponsePassPriorityEventArgs : ExtendedEventArgs {

    public ResponsePassPriorityEventArgs() {
        Event_id = Constants.SMSG_PASS_PRIORITY;
    }
}

public class ResponsePassPriority : BaseNetworkResponse {
    private int playerId;

    public ResponsePassPriority() {
        Response_id = Constants.SMSG_PASS_PRIORITY;
    }

    protected override void ParseResponseData() {
    }

    public override ExtendedEventArgs Process() {
        ResponsePassPriorityEventArgs args = new ResponsePassPriorityEventArgs();
        return args;
    }
}
