using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class ResponseDrawCardEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public List<CardData> Cards { get; set; }

    public ResponseDrawCardEventArgs() {
        Event_id = Constants.SMSG_CARD_DRAW;
        Cards = new List<CardData>();
    }
}

public class ResponseDrawCard : BaseNetworkResponse {
    private List<CardData> cards = new List<CardData>();

    public ResponseDrawCard() {
        Response_id = Constants.SMSG_CARD_DRAW;
    }

    protected override void ParseResponseData() {
        short cardCount = DataReader.ReadShort(DataStream);
        for (int i = 0; i < cardCount; i++) {
            CardData card = new CardData();
            card.Id = DataReader.ReadInt(DataStream);
            card.Suit = DataReader.ReadInt(DataStream);
            card.Value = DataReader.ReadInt(DataStream);
            card.Type = DataReader.ReadInt(DataStream);
            cards.Add(card);
        }
    }

    public override ExtendedEventArgs Process() {
        ResponseDrawCardEventArgs args = new ResponseDrawCardEventArgs();
        args.Status = status;
        args.Cards = cards;
        return args;
    }
}
