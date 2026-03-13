using UnityEngine;

public class ResponseNotifyPlayerSelectEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public string Message { get; set; }
    public int PlayerId { get; set; }
    public int ChampionId { get; set; }

    public ResponseNotifyPlayerSelectEventArgs() {
        Event_id = Constants.SMSG_NOTIFY_PLAYER_SELECT;
    }
}

public class ResponseNotifyPlayerSelect : BaseNetworkResponse {
    private int playerId;
    private int championId;

    public ResponseNotifyPlayerSelect() {
        Response_id = Constants.SMSG_NOTIFY_PLAYER_SELECT;
    }
    
    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        championId = DataReader.ReadInt(DataStream);
    }
    
    public override ExtendedEventArgs Process() {
        ResponseNotifyPlayerSelectEventArgs args = new ResponseNotifyPlayerSelectEventArgs();
        args.Status = status;
        args.Message = message;
        args.PlayerId = playerId;
        args.ChampionId = championId;
        return args;
    }
}
