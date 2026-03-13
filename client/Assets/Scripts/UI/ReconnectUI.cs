using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// This UI acts as the "League of Legends Reconnect Screen".
/// Players are locked here if they log into an account that is currently inside an active game.
/// </summary>
public class ReconnectUI : MonoBehaviour
{
    [SerializeField] private Button reconnectButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        if (reconnectButton != null)
        {
            reconnectButton.onClick.AddListener(OnReconnectClicked);
        }

        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.AddCallback(Constants.SMSG_RECONNECT, OnReconnectResponse);
        }
        
        SetStatus("The game has crashed or you disconnected. Please reconnect to continue.");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_RECONNECT);
        }
    }

    private void OnReconnectClicked()
    {
        SetStatus("Syncing game state...");
        if (reconnectButton != null) reconnectButton.interactable = false;

        // Request the massive state sync from the server using the session token
        RequestReconnect request = new RequestReconnect();
        request.Send(NetworkManager.Instance.lastUsername, NetworkManager.Instance.lastSessionToken);
        NetworkManager.Instance.SendRequest(request);
    }

    private void OnReconnectResponse(ExtendedEventArgs eventArgs)
    {
        ResponseReconnectEventArgs args = eventArgs as ResponseReconnectEventArgs;
        if (args != null)
        {
            if (args.Status == Constants.SUCCESS)
            {
                SetStatus("State synced successfully! Loading match...");
                Debug.Log($"Reconnecting to match in room {args.RoomId}. Board state has been initialized.");

                // Load the actual Game scene
                SceneManager.LoadScene("Game");
            }
            else
            {
                SetStatus("Failed to reconnect. The match may have ended.");
                if (reconnectButton != null) reconnectButton.interactable = true;

                // If the game ended while we were offline, return to lobby
                SceneManager.LoadScene("Lobby");
            }
        }
    }

    private void SetStatus(string msg)
    {
        if (statusText != null)
        {
            statusText.text = msg;
        }
    }
}
