using UnityEngine;

public class ResponseFieldToHandEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int CasterId { get; set; }
    public int CardId { get; set; }
    public int Suit { get; set; }
    public int Value { get; set; }
    public int CardType { get; set; }

    public ResponseFieldToHandEventArgs() {
        Event_id = Constants.SMSG_FIELD_TO_HAND;
    }
}

public class ResponseFieldToHand : BaseNetworkResponse {
    private int casterId;
    private int cardId;
    private int suit;
    private int value;
    private int cardType;

    public ResponseFieldToHand() {
        Response_id = Constants.SMSG_FIELD_TO_HAND;
    }

    protected override void ParseResponseData() {
        casterId = DataReader.ReadInt(DataStream);
        cardId = DataReader.ReadInt(DataStream);
        suit = DataReader.ReadInt(DataStream);
        value = DataReader.ReadInt(DataStream);
        cardType = DataReader.ReadInt(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseFieldToHandEventArgs args = new ResponseFieldToHandEventArgs();
        args.Status = status;
        args.CasterId = casterId;
        args.CardId = cardId;
        args.Suit = suit;
        args.Value = value;
        args.CardType = cardType;
        
        // Also call base utility to ensure Status is set via reflection if needed
        SetBaseEventArgs(args);
        
        return args;
    }
}
