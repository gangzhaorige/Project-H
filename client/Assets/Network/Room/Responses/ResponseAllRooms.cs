using UnityEngine;
using System.Collections.Generic;

public class RoomInfo {
    public string Id;
    public string Name;
    public int PlayerCount;
}

public class ResponseAllRoomsEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public List<RoomInfo> Rooms { get; set; }

    public ResponseAllRoomsEventArgs() {
        Event_id = Constants.SMSG_ALL_ROOMS;
        Rooms = new List<RoomInfo>();
    }
}

public class ResponseAllRooms : BaseNetworkResponse {
    private List<RoomInfo> rooms;

    public ResponseAllRooms() {
        Response_id = Constants.SMSG_ALL_ROOMS;
        rooms = new List<RoomInfo>();
    }
    
    protected override void ParseResponseData() {
        short roomCount = DataReader.ReadShort(DataStream);
        for (int i = 0; i < roomCount; i++) {
            RoomInfo info = new RoomInfo();
            info.Id = DataReader.ReadString(DataStream);
            info.Name = DataReader.ReadString(DataStream);
            info.PlayerCount = DataReader.ReadInt(DataStream);
            rooms.Add(info);
        }
    }
    
    public override ExtendedEventArgs Process() {
        ResponseAllRoomsEventArgs args = new ResponseAllRoomsEventArgs();
        args.Status = status;
        args.Rooms = rooms;
        return args;
    }
}
