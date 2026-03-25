using UnityEngine;

public class ResponseTimerStartEventArgs : ExtendedEventArgs {
    public int PlayerId { get; set; }
    public int Seconds { get; set; }
    public string Message { get; set; }
    public int RequiredCardType { get; set; }

    public ResponseTimerStartEventArgs() {
        Event_id = Constants.SMSG_RESPONSE_TIMER_START;
    }
}

public class ResponseTimerStart : BaseNetworkResponse {
    private int playerId;
    private int seconds;
    private string instruction;
    private int requiredCardType;

    public ResponseTimerStart() {
        Response_id = Constants.SMSG_RESPONSE_TIMER_START;
    }

    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        seconds = DataReader.ReadInt(DataStream);
        instruction = DataReader.ReadString(DataStream);
        requiredCardType = DataReader.ReadInt(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseTimerStartEventArgs args = new ResponseTimerStartEventArgs();
        args.PlayerId = playerId;
        args.Seconds = seconds;
        args.Message = instruction;
        args.RequiredCardType = requiredCardType;
        return args;
    }
}
