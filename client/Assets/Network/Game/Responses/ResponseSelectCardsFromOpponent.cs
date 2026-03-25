using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class ResponseSelectCardsFromOpponentEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int TargetPlayerId { get; set; }

    public int TargetHandSize { get; set; }
    public int Amount { get; set; }
    public int Duration { get; set; }
    public string Message { get; set; }

    public ResponseSelectCardsFromOpponentEventArgs() {
        Event_id = Constants.SMSG_SELECT_CARDS_FROM_OPPONENT;
    }
}

public class ResponseSelectCardsFromOpponent : BaseNetworkResponse {
    private int targetPlayerId;
    private int amount;
    private int duration;
    private string message;

    private int targetHandSize;

    public ResponseSelectCardsFromOpponent() {
        Response_id = Constants.SMSG_SELECT_CARDS_FROM_OPPONENT;
    }

    protected override void ParseResponseData() {
        targetHandSize = DataReader.ReadInt(DataStream);
        targetPlayerId = DataReader.ReadInt(DataStream);
        amount = DataReader.ReadInt(DataStream);
        duration = DataReader.ReadInt(DataStream);
        message = DataReader.ReadString(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseSelectCardsFromOpponentEventArgs args = new ResponseSelectCardsFromOpponentEventArgs();
        args.Status = status;
        args.TargetPlayerId = targetPlayerId;
        args.Amount = amount;
        args.Duration = duration;
        args.Message = message;
        args.TargetHandSize = targetHandSize;
        return args;
    }
}
