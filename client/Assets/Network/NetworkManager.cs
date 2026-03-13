using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static MessageQueue;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(ConnectionManager), typeof(MessageQueue))]
public class NetworkManager : MonoBehaviour
{


    public static NetworkManager Instance { get; private set; }

    public float PingMilliseconds { get; private set; }

    /// <summary>
    /// Returns true if the client is currently connected to the server.
    /// </summary>
    public bool IsConnected => connectionManager != null && connectionManager.IsConnected;

    private float lastHeartbeatSentTime;
    private ConnectionManager connectionManager;
    private MessageQueue messageQueue;
    private Coroutine heartbeatCoroutine;

    // Credentials for silent reconnection
    public string lastUsername { get; set; }
    public string lastSessionToken { get; set; }

    private void Awake()
    {
        // --- Singleton Pattern Implementation ---
        if (Instance != null && Instance != this)
        {
            // If another instance already exists, destroy this one.
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes.

        // --- Component Caching ---
        // Get references to required components that are guaranteed to exist by [RequireComponent].
        connectionManager = GetComponent<ConnectionManager>();
        messageQueue = GetComponent<MessageQueue>();

        // --- Table Initialization ---
        NetworkRequestTable.Init();
        NetworkResponseTable.Init();
        connectionManager.SetupSocket();
        
    }

    private void Start()
    {
        AddCallback(Constants.SMSG_HEARTBEAT, OnHeartbeatResponse);
        StartHeartbeatLoop(); // Start the heartbeat process.
        
        if (IsConnected && SceneManager.GetActiveScene().name != "Login")
        {
            SceneManager.LoadScene("Login");
        }
    }

    private bool isReconnecting = false;

    private void Update()
    {
        // Reset ping if the connection is lost.
        if (!IsConnected)
        {
            PingMilliseconds = 0;
            if (!isReconnecting)
            {
                StartCoroutine(ReconnectRoutine());
            }
        }
    }

    private IEnumerator ReconnectRoutine()
    {
        isReconnecting = true;
        Debug.Log("Connection lost. Attempting to reconnect...");
        
        while (!IsConnected)
        {
            connectionManager.SetupSocket();
            yield return new WaitForSeconds(3f); // Wait 3 seconds before trying again
        }

        Debug.Log("Reconnected successfully. Silent re-authentication in progress...");
        
        if (!string.IsNullOrEmpty(lastSessionToken)) {
            // Use session token to restore identity and room state in one go
            RequestReconnect reconReq = new RequestReconnect();
            reconReq.Send(lastUsername, lastSessionToken);
            SendRequest(reconReq);
        }

        isReconnecting = false;
    }

    public void StartHeartbeatLoop()
    {
        if (heartbeatCoroutine == null)
        {
            heartbeatCoroutine = StartCoroutine(RequestHeartbeatRoutine(1.0f));
        }
    }


    public void StopHeartbeatLoop()
    {
        if (heartbeatCoroutine != null)
        {
            StopCoroutine(heartbeatCoroutine);
            heartbeatCoroutine = null;
        }
    }

    private void OnHeartbeatResponse(ExtendedEventArgs args)
    {
        
    }

    private IEnumerator RequestHeartbeatRoutine(float interval)
    {
        while (true)
        {
            try
            {
                if (IsConnected)
                {
                    lastHeartbeatSentTime = Time.realtimeSinceStartup;
                    RequestHeartBeat request = new RequestHeartBeat();
                    request.Send();
                    connectionManager.Send(request);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Heartbeat error: " + e.Message);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void SendRequest(NetworkRequest request)
    {
        if (connectionManager != null)
        {
            connectionManager.Send(request);
        }
    }

    public void AddCallback(int eventId, Callback callback)
    {
        messageQueue.AddCallback(eventId, callback);
    }

	public void RemoveCallback(int eventId) {
        messageQueue.RemoveCallback(eventId);
	}
}
