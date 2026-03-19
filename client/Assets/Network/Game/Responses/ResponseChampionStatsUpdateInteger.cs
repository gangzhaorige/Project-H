using UnityEngine;

public class ResponseChampionStatsUpdateIntegerEventArgs : ExtendedEventArgs {
    public int ChampionId { get; set; }
    public int StatId { get; set; }
    public int Value { get; set; }

    public ResponseChampionStatsUpdateIntegerEventArgs() {
        Event_id = Constants.SMSG_CHAMPION_STATS_UPDATE_INTEGER;
    }
}

public class ResponseChampionStatsUpdateInteger : BaseNetworkResponse {
    private int championId;
    private int statId;
    private int value;

    public ResponseChampionStatsUpdateInteger() {
        Response_id = Constants.SMSG_CHAMPION_STATS_UPDATE_INTEGER;
    }

    protected override void ParseResponseData() {
        championId = DataReader.ReadInt(DataStream);
        statId = DataReader.ReadInt(DataStream);
        value = DataReader.ReadInt(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseChampionStatsUpdateIntegerEventArgs args = new ResponseChampionStatsUpdateIntegerEventArgs();
        args.ChampionId = championId;
        args.StatId = statId;
        args.Value = value;
        return args;
    }
}
