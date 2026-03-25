using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class ResponseMoveCardEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int CasterId { get; set; } // Receiver
    public int TargetId { get; set; } // Source/Loser
    public bool ShowDetails { get; set; }
    public List<CardData> Cards { get; set; }

    public ResponseMoveCardEventArgs() {
        Event_id = Constants.SMSG_MOVE_CARD;
        Cards = new List<CardData>();
    }
}

public class ResponseMoveCard : BaseNetworkResponse {
    private int casterId;
    private int targetId;
    private bool showDetails;
    private List<CardData> cards = new List<CardData>();

    public ResponseMoveCard() {
        Response_id = Constants.SMSG_MOVE_CARD;
    }

    protected override void ParseResponseData() {
        casterId = DataReader.ReadInt(DataStream);
        targetId = DataReader.ReadInt(DataStream);
        showDetails = DataReader.ReadBool(DataStream);
        
        short cardCount = DataReader.ReadShort(DataStream);
        for (int i = 0; i < cardCount; i++) {
            CardData card = new CardData();
            card.Id = DataReader.ReadInt(DataStream);
            card.Suit = DataReader.ReadInt(DataStream);
            card.Value = DataReader.ReadInt(DataStream);
            card.Type = DataReader.ReadString(DataStream);
            cards.Add(card);
        }
    }

    public override ExtendedEventArgs Process() {
        ResponseMoveCardEventArgs args = new ResponseMoveCardEventArgs();
        args.Status = status;
        args.CasterId = casterId;
        args.TargetId = targetId;
        args.ShowDetails = showDetails;
        args.Cards = cards;
        return args;
    }
}
