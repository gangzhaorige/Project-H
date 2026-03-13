using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestLogin : NetworkRequest
{
    public RequestLogin()
    {
        Request_id = Constants.CMSG_AUTH;
    }

    public void Send(string username, string password)
    {
        Packet = new GamePacket(Request_id);
        Packet.AddString(username);
        Packet.AddString(password);
    }
}
