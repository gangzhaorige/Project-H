using UnityEngine;

public class ResponseNotifyPlayerPickEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string Message { get; set; }
    public int PlayerId { get; set; }
    public int ChampionId { get; set; }

    public ResponseNotifyPlayerPickEventArgs() {
        Event_id = Constants.SMSG_NOTIFY_PLAYER_PICK;
    }
}

public class ResponseNotifyPlayerPick : BaseNetworkResponse {
    private int playerId;
    private int championId;

    public ResponseNotifyPlayerPick() {
        Response_id = Constants.SMSG_NOTIFY_PLAYER_PICK;
    }
    
    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        championId = DataReader.ReadInt(DataStream);
    }
    
    public override ExtendedEventArgs Process() {
        ResponseNotifyPlayerPickEventArgs args = new ResponseNotifyPlayerPickEventArgs();
        args.Status = status;
        args.Message = message;
        args.PlayerId = playerId;
        args.ChampionId = championId;
        return args;
    }
}
