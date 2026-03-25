using UnityEngine;
using System.Collections.Generic;

public class ResponseUpdatePlayerOrderEventArgs : ExtendedEventArgs {
    public List<int> PlayerOrder { get; set; }

    public ResponseUpdatePlayerOrderEventArgs() {
        Event_id = Constants.SMSG_UPDATE_PLAYER_ORDER;
        PlayerOrder = new List<int>();
    }
}

public class ResponseUpdatePlayerOrder : BaseNetworkResponse {
    private List<int> playerOrder = new List<int>();

    public ResponseUpdatePlayerOrder() {
        Response_id = Constants.SMSG_UPDATE_PLAYER_ORDER;
    }

    protected override void ParseResponseData() {
        short count = DataReader.ReadShort(DataStream);
        for (int i = 0; i < count; i++) {
            playerOrder.Add(DataReader.ReadInt(DataStream));
        }
    }

    public override ExtendedEventArgs Process() {
        ResponseUpdatePlayerOrderEventArgs args = new ResponseUpdatePlayerOrderEventArgs();
        args.PlayerOrder = playerOrder;
        return args;
    }
}
