using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class ResponsePlayCardEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int PlayerId { get; set; }
    public int CardId { get; set; }
    public int Suit { get; set; }
    public int Value { get; set; }
    public string CardType { get; set; }
    public List<int> TargetIds { get; set; }

    public ResponsePlayCardEventArgs() {
        Event_id = Constants.SMSG_PLAY_CARD;
        TargetIds = new List<int>();
    }
}

public class ResponsePlayCard : BaseNetworkResponse {
    private int playerId;
    private int cardId;
    private int suit;
    private int value;
    private string cardType;
    private List<int> targetIds = new List<int>();

    public ResponsePlayCard() {
        Response_id = Constants.SMSG_PLAY_CARD;
    }

    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        cardId = DataReader.ReadInt(DataStream);
        suit = DataReader.ReadInt(DataStream);
        value = DataReader.ReadInt(DataStream);
        cardType = DataReader.ReadString(DataStream);

        short targetCount = DataReader.ReadShort(DataStream);
        for (int i = 0; i < targetCount; i++) {
            targetIds.Add(DataReader.ReadInt(DataStream));
        }
    }

    public override ExtendedEventArgs Process() {
        ResponsePlayCardEventArgs args = new ResponsePlayCardEventArgs();
        args.Status = status;
        args.PlayerId = playerId;
        args.CardId = cardId;
        args.Suit = suit;
        args.Value = value;
        args.CardType = cardType;
        args.TargetIds = targetIds;
        return args;
    }
}
