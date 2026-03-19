using UnityEngine;

public class ResponseTimerCancelEventArgs : ExtendedEventArgs {
    public ResponseTimerCancelEventArgs() {
        Event_id = Constants.SMSG_RESPONSE_TIMER_CANCEL;
    }
}

public class ResponseTimerCancel : BaseNetworkResponse {
    public ResponseTimerCancel() {
        Response_id = Constants.SMSG_RESPONSE_TIMER_CANCEL;
    }

    protected override void ParseResponseData() {
        // No extra data
    }

    public override ExtendedEventArgs Process() {
        return new ResponseTimerCancelEventArgs();
    }
}
