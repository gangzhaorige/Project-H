using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A sample game scene UI controller to handle real-time connection stats.
/// Demonstrates catching SMSG_MATCH_STATE to see who drops out of the game.
/// </summary>
public class SampleGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playersConnectedText;
    [SerializeField] private TextMeshProUGUI updatesCountText;
    [SerializeField] private TextMeshProUGUI matchStatusText;
    [SerializeField] private Button endGameButton;

    private int updateCounter = 0;

    private void Start()
    {
        if (endGameButton != null)
        {
            endGameButton.onClick.AddListener(OnEndGameClicked);
        }

        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.AddCallback(Constants.SMSG_MATCH_STATE, OnMatchStateUpdate);
            NetworkManager.Instance.AddCallback(Constants.SMSG_GAME_END, OnGameEnded);
        }

        SetStatus("Match is live.");
        UpdatePlayersText(1, 1); // Defaults
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_MATCH_STATE);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_GAME_END);
        }
    }

    private void OnEndGameClicked()
    {
        SetStatus("Requesting game end...");
        if (endGameButton != null) endGameButton.interactable = false;

        RequestEndGame request = new RequestEndGame();
        request.Send();
        NetworkManager.Instance.SendRequest(request);
    }

    private void OnMatchStateUpdate(ExtendedEventArgs eventArgs)
    {
        updateCounter++;
        if (updatesCountText != null)
        {
            updatesCountText.text = $"Updates Received: {updateCounter}";
        }

        ResponseMatchStateEventArgs args = eventArgs as ResponseMatchStateEventArgs;
        if (args != null && args.Status == Constants.SUCCESS)
        {
            UpdatePlayersText(args.ConnectedPlayers, args.TotalPlayers);

            if (args.ConnectedPlayers < args.TotalPlayers)
            {
                SetStatus($"Someone disconnected! Waiting for reconnect... ({args.ConnectedPlayers}/{args.TotalPlayers})");
            }
            else
            {
                SetStatus("All players connected.");
            }
        }
    }

    private void OnGameEnded(ExtendedEventArgs eventArgs)
    {
        ResponseGameEndEventArgs args = eventArgs as ResponseGameEndEventArgs;
        if (args != null)
        {
            Debug.Log("Game ended, returning to lobby.");
            // UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }
    }

    private void UpdatePlayersText(int connected, int total)
    {
        if (playersConnectedText != null)
        {
            playersConnectedText.text = $"Players Connected: {connected} / {total}";
        }
    }

    private void SetStatus(string msg)
    {
        if (matchStatusText != null)
        {
            matchStatusText.text = msg;
        }
        Debug.Log("[GameUI] " + msg);
    }
}
