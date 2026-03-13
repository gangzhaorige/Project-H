using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

public class ConnectionManager : MonoBehaviour {
    
    private GameObject mainObject;
    private TcpClient mySocket;
    private NetworkStream theStream;
    private bool socketReady = false;
    
    // --- NEW: IsConnected Property ---
    // This allows other scripts (like NetworkManager) to safely check the
    // current connection status without accessing private variables.
    public bool IsConnected {
        get {
            // A connection is active if our flag is set AND the underlying
            // TCP client reports that it is still connected.
            return socketReady && mySocket != null && mySocket.Connected;
        }
    }
    // ---------------------------------
    
    void Awake() {
        mainObject = GameObject.Find("Networking");
    }
    
    // Use this for initialization
    void Start () {
        SetupSocket();
    }

    public void SetupSocket() {
        if (IsConnected) {
            Debug.Log("Already Connected");
            return;
        }
        try {
            if (mySocket != null) {
                try { mySocket.Close(); } catch { }
            }
            mySocket = new TcpClient (Constants.REMOTE_HOST, Constants.REMOTE_PORT);
            theStream = mySocket.GetStream();
            socketReady = true;
            Debug.Log("Connected");
        } catch (Exception e) {
            socketReady = false;
            Debug.Log("Socket error: " + e);
        }
    }

    public void ReadSocket() {
        if (!IsConnected) { // It's good practice to use the new property here too!
            return;
        }
        try {
            if (theStream.DataAvailable) {
                byte[] buffer = new byte[2];
                theStream.Read(buffer, 0, 2);
                short bufferSize = BitConverter.ToInt16(buffer, 0);
                buffer = new byte[bufferSize];
                theStream.Read(buffer, 0, bufferSize);
                MemoryStream dataStream = new MemoryStream(buffer);
                short response_id = DataReader.ReadShort(dataStream);
                NetworkResponse response = NetworkResponseTable.Get(response_id);
                if (response != null) {
                    response.DataStream = dataStream;
                    response.Parse();
                    ExtendedEventArgs args = response.Process();
                    if (args != null) {
                        MessageQueue msgQueue = mainObject.GetComponent<MessageQueue>();
                        msgQueue.AddMessage(args.Event_id, args);
                    }
                }
            }
        } catch (Exception e) {
            Debug.Log("Socket read error: " + e.Message);
            socketReady = false;
        }
    }

    public void CloseSocket() {
        if (!socketReady) {
            return;
        }
        mySocket.Close();
        socketReady = false;
    }
    
    public void Send(NetworkRequest request) {
        if (!IsConnected) { // And here!
            return;
        }
        try {
            GamePacket packet = request.Packet;
            byte[] bytes = packet.GetBytes();
            theStream.Write(bytes, 0, bytes.Length);
            if (request.Request_id != Constants.CMSG_HEARTBEAT) {
                Debug.Log("Sent Request No. " + request.Request_id + " [" + request.ToString() + "]");
            }
        } catch (Exception e) {
            Debug.Log("Socket send error: " + e.Message);
            socketReady = false;
        }
    }
    
    // Update is called once per frame
    void Update () {
        ReadSocket();
    }
}