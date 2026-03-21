using UnityEngine;
using System.Collections.Generic;

public class ResponseDiscardCardEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int PlayerId { get; set; }
    public int CardId { get; set; }
    public int Suit { get; set; }
    public int Value { get; set; }
    public string CardType { get; set; }
    public bool ShowJudge { get; set; }
    public bool JudgeResult { get; set; }

    public ResponseDiscardCardEventArgs() {
        Event_id = Constants.SMSG_DISCARD_CARDS;
    }
}

public class ResponseDiscardCard : BaseNetworkResponse {
    private int playerId;
    private int cardId;
    private int suit;
    private int value;
    private string cardType;
    private bool showJudge;
    private bool judgeResult;

    public ResponseDiscardCard() {
        Response_id = Constants.SMSG_DISCARD_CARDS;
    }

    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        cardId = DataReader.ReadInt(DataStream);
        suit = DataReader.ReadInt(DataStream);
        value = DataReader.ReadInt(DataStream);
        cardType = DataReader.ReadString(DataStream);
        showJudge = DataReader.ReadBool(DataStream);
        judgeResult = DataReader.ReadBool(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseDiscardCardEventArgs args = new ResponseDiscardCardEventArgs();
        args.Status = status;
        args.PlayerId = playerId;
        args.CardId = cardId;
        args.Suit = suit;
        args.Value = value;
        args.CardType = cardType;
        args.ShowJudge = showJudge;
        args.JudgeResult = judgeResult;
        return args;
    }
}
