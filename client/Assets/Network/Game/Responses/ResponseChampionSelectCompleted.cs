using UnityEngine;

public class ResponseChampionSelectCompletedEventArgs : ExtendedEventArgs {
    public short Status { get; set; }

    public ResponseChampionSelectCompletedEventArgs() {
        Event_id = Constants.SMSG_CHAMPION_SELECT_COMPLETED;
    }
}

public class ResponseChampionSelectCompleted : BaseNetworkResponse {
    public ResponseChampionSelectCompleted() {
        Response_id = Constants.SMSG_CHAMPION_SELECT_COMPLETED;
    }
    
    protected override void ParseResponseData() {
    }
    
    public override ExtendedEventArgs Process() {
        ResponseChampionSelectCompletedEventArgs args = new ResponseChampionSelectCompletedEventArgs();
        args.Status = status;
        return args;
    }
}
