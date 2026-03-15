using UnityEngine;

public class ResponseDrawCardOtherEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int PlayerId { get; set; }
    public int CardCount { get; set; }

    public ResponseDrawCardOtherEventArgs() {
        Event_id = Constants.SMSG_CARD_DRAW_OTHER;
    }
}

public class ResponseDrawCardOther : BaseNetworkResponse {
    private int playerId;
    private int cardCount;

    public ResponseDrawCardOther() {
        Response_id = Constants.SMSG_CARD_DRAW_OTHER;
    }

    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        cardCount = DataReader.ReadInt(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseDrawCardOtherEventArgs args = new ResponseDrawCardOtherEventArgs();
        args.Status = status;
        args.PlayerId = playerId;
        args.CardCount = cardCount;
        return args;
    }
}
