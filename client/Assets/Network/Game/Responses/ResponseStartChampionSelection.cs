using UnityEngine;

public class ResponseStartChampionSelectionEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string Message { get; set; }

    public ResponseStartChampionSelectionEventArgs() {
        Event_id = Constants.SMSG_START_CHAMPION_SELECTION;
    }
}

public class ResponseStartChampionSelection : BaseNetworkResponse {
    public ResponseStartChampionSelection() {
        Response_id = Constants.SMSG_START_CHAMPION_SELECTION;
    }
    
    protected override void ParseResponseData() {
    }
    
    public override ExtendedEventArgs Process() {
        ResponseStartChampionSelectionEventArgs args = new ResponseStartChampionSelectionEventArgs();
        args.Status = status;
        args.Message = message;
        return args;
    }
}
