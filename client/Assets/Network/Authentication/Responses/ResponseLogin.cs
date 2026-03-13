using UnityEngine;
using System;
using System.Collections.Generic;

public class ResponseLoginEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string Message { get; set; }
    public int PlayerId { get; set; }
    public string Username { get; set; }
    public string RoomId { get; set; }
    public string SessionToken { get; set; }
    public bool InGame { get; set; }

    public ResponseLoginEventArgs() {
        Event_id = Constants.SMSG_AUTH;
    }
}

public class ResponseLogin : BaseNetworkResponse {

    private int playerId;
    private string username;
    private string roomId;
    private string sessionToken;
    private bool inGame;

    public ResponseLogin()
    {
        Response_id = Constants.SMSG_AUTH;
    }
    
    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        username = DataReader.ReadString(DataStream);
        roomId = DataReader.ReadString(DataStream);
        sessionToken = DataReader.ReadString(DataStream);
        inGame = DataReader.ReadBool(DataStream);
    }
    
    public override ExtendedEventArgs Process() {
        ResponseLoginEventArgs args = new ResponseLoginEventArgs();
        args.Status = status;
        args.Message = message;
        args.PlayerId = playerId;
        args.Username = username;
        args.RoomId = roomId;
        args.SessionToken = sessionToken;
        args.InGame = inGame;
        
        // Also update local constants
        Constants.USER_ID = playerId;
        Constants.ROOM_ID = roomId;
        
        return args;
    }
}
