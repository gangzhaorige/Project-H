using UnityEngine;
using System.Collections.Generic;

public class ChampionSetupInfo {
    public int ChampionId;
    public string ChampionName;
    public int MaxHP;
    public int CurHP;
    public int PathId;
    public string Element;
    public int Attack;
    public int AttackRange;
}

public class PlayerSetupInfo {
    public int PlayerId;
    public string Username;
    public int Team;
    public int PlayerIndex;
    public ChampionSetupInfo Champion;
}

public class ResponseGameSetupEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public List<PlayerSetupInfo> Players { get; set; }

    public ResponseGameSetupEventArgs() {
        Event_id = Constants.SMSG_GAME_SETUP;
        Players = new List<PlayerSetupInfo>();
    }
}

public class ResponseGameSetup : BaseNetworkResponse {
    private List<PlayerSetupInfo> players = new List<PlayerSetupInfo>();

    public ResponseGameSetup() {
        Response_id = Constants.SMSG_GAME_SETUP;
    }

    protected override void ParseResponseData() {
        short playerCount = DataReader.ReadShort(DataStream);
        for (int i = 0; i < playerCount; i++) {
            PlayerSetupInfo player = new PlayerSetupInfo();
            player.PlayerId = DataReader.ReadInt(DataStream);
            player.Username = DataReader.ReadString(DataStream);
            player.Team = DataReader.ReadInt(DataStream);
            player.PlayerIndex = DataReader.ReadInt(DataStream);

            int champId = DataReader.ReadInt(DataStream);
            if (champId != -1) {
                player.Champion = new ChampionSetupInfo();
                player.Champion.ChampionId = champId;
                player.Champion.ChampionName = DataReader.ReadString(DataStream);
                player.Champion.MaxHP = DataReader.ReadInt(DataStream);
                player.Champion.CurHP = DataReader.ReadInt(DataStream);
                player.Champion.PathId = DataReader.ReadInt(DataStream);
                player.Champion.Element = DataReader.ReadString(DataStream);
                player.Champion.Attack = DataReader.ReadInt(DataStream);
                player.Champion.AttackRange = DataReader.ReadInt(DataStream);
            }
            players.Add(player);
        }
    }

    public override ExtendedEventArgs Process() {
        ResponseGameSetupEventArgs args = new ResponseGameSetupEventArgs();
        args.Status = status;
        args.Players = players;
        return args;
    }
}
