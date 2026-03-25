using UnityEngine;

public class ResponseJudgementEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int CardId { get; set; }
    public int Suit { get; set; }
    public int Value { get; set; }
    public int CardType { get; set; }
    public bool JudgeResult { get; set; }

    public ResponseJudgementEventArgs() {
        Event_id = Constants.SMSG_JUDGE;
    }
}

public class ResponseJudgement : BaseNetworkResponse {
    private int cardId;
    private int suit;
    private int value;
    private int cardType;
    private bool judgeResult;

    public ResponseJudgement() {
        Response_id = Constants.SMSG_JUDGE;
    }

    protected override void ParseResponseData() {
        cardId = DataReader.ReadInt(DataStream);
        suit = DataReader.ReadInt(DataStream);
        value = DataReader.ReadInt(DataStream);
        cardType = DataReader.ReadInt(DataStream);
        judgeResult = DataReader.ReadBool(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseJudgementEventArgs args = new ResponseJudgementEventArgs();
        args.Status = status;
        args.CardId = cardId;
        args.Suit = suit;
        args.Value = value;
        args.CardType = cardType;
        args.JudgeResult = judgeResult;
        return args;
    }
}
