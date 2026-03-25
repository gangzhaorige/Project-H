using UnityEngine;

public class ResponseSelectCardsEventArgs : ExtendedEventArgs {
    public short Status { get; set; }

    public ResponseSelectCardsEventArgs() {
        Event_id = Constants.SMSG_SELECT_CARDS;
    }
}

public class ResponseSelectCards : BaseNetworkResponse {

    public ResponseSelectCards() {
        Response_id = Constants.SMSG_SELECT_CARDS;
    }

    protected override void ParseResponseData() {
        // Only status is expected
    }

    public override ExtendedEventArgs Process() {
        ResponseSelectCardsEventArgs args = new ResponseSelectCardsEventArgs();
        args.Status = status;
        return args;
    }
}
