using UnityEngine;

public class ResponseSwapFieldHandEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int CasterId { get; set; }
    
    // Swapped Card (Field -> Hand)
    public int SwappedCardId { get; set; }
    public int SwappedSuit { get; set; }
    public int SwappedValue { get; set; }
    public int SwappedCardType { get; set; }

    // Played Card (Hand -> Field)
    public int PlayedCardId { get; set; }
    public int PlayedSuit { get; set; }
    public int PlayedValue { get; set; }
    public int PlayedCardType { get; set; }

    public bool JudgeResult { get; set; }

    public ResponseSwapFieldHandEventArgs() {
        Event_id = Constants.SMSG_SWAP_FIELD_HAND;
    }
}

public class ResponseSwapFieldHand : BaseNetworkResponse {
    private int casterId;
    
    private int sCardId, sSuit, sValue;
    private int sType;

    private int pCardId, pSuit, pValue;
    private int pType;

    private bool judgeResult;

    public ResponseSwapFieldHand() {
        Response_id = Constants.SMSG_SWAP_FIELD_HAND;
    }

    protected override void ParseResponseData() {
        casterId = DataReader.ReadInt(DataStream);

        // Swapped
        sCardId = DataReader.ReadInt(DataStream);
        sSuit = DataReader.ReadInt(DataStream);
        sValue = DataReader.ReadInt(DataStream);
        sType = DataReader.ReadInt(DataStream);

        // Played
        pCardId = DataReader.ReadInt(DataStream);
        pSuit = DataReader.ReadInt(DataStream);
        pValue = DataReader.ReadInt(DataStream);
        pType = DataReader.ReadInt(DataStream);

        judgeResult = DataReader.ReadBool(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseSwapFieldHandEventArgs args = new ResponseSwapFieldHandEventArgs();
        args.Status = status;
        args.CasterId = casterId;

        args.SwappedCardId = sCardId;
        args.SwappedSuit = sSuit;
        args.SwappedValue = sValue;
        args.SwappedCardType = sType;

        args.PlayedCardId = pCardId;
        args.PlayedSuit = pSuit;
        args.PlayedValue = pValue;
        args.PlayedCardType = pType;

        args.JudgeResult = judgeResult;
        return args;
    }
}
