using UnityEngine;
using System.Collections.Generic;

public class PlayerReadyInfo {
    public int PlayerId;
    public string Username;
    public int Team;
}

public class ResponseChampionSelectReadyEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public List<PlayerReadyInfo> Players { get; set; }
    public List<int> ChampionPool { get; set; }

    public ResponseChampionSelectReadyEventArgs() {
        Event_id = Constants.SMSG_CHAMPION_SELECT_READY;
        Players = new List<PlayerReadyInfo>();
        ChampionPool = new List<int>();
    }
}

public class ResponseChampionSelectReady : BaseNetworkResponse {
    private List<PlayerReadyInfo> players = new List<PlayerReadyInfo>();
    private List<int> championPool = new List<int>();

    public ResponseChampionSelectReady() {
        Response_id = Constants.SMSG_CHAMPION_SELECT_READY;
    }
    
    protected override void ParseResponseData() {
        // Players
        short playerCount = DataReader.ReadShort(DataStream);
        for (int i = 0; i < playerCount; i++) {
            PlayerReadyInfo info = new PlayerReadyInfo();
            info.PlayerId = DataReader.ReadInt(DataStream);
            info.Username = DataReader.ReadString(DataStream);
            info.Team = DataReader.ReadInt(DataStream);
            players.Add(info);
        }

        // Champion Pool
        short championCount = DataReader.ReadShort(DataStream);
        for (int i = 0; i < championCount; i++) {
            championPool.Add(DataReader.ReadInt(DataStream));
        }
    }
    
    public override ExtendedEventArgs Process() {
        ResponseChampionSelectReadyEventArgs args = new ResponseChampionSelectReadyEventArgs();
        args.Status = status;
        args.Players = players;
        args.ChampionPool = championPool;
        return args;
    }
}
