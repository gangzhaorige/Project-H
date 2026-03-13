using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        // Subscribe to the login response from the server
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.AddCallback(Constants.SMSG_AUTH, OnLoginResponse);
        }
        else
        {
            Debug.LogWarning("NetworkManager Instance is null. Ensure a NetworkManager exists in the scene.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe when this UI is destroyed
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_AUTH);
        }
    }

    public void OnLoginButtonClicked()
    {
        string username = usernameInput != null ? usernameInput.text : "";
        string password = passwordInput != null ? passwordInput.text : "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            if (statusText != null)
            {
                statusText.text = "Username and password cannot be empty.";
                statusText.color = Color.red;
            }
            return;
        }

        if (statusText != null)
        {
            statusText.text = "Logging in...";
            statusText.color = Color.white;
        }

        if (loginButton != null)
        {
            loginButton.interactable = false;
        }

        // Create the login request and send it
        RequestLogin request = new RequestLogin();
        request.Send(username, password);

        if (NetworkManager.Instance != null && NetworkManager.Instance.IsConnected)
        {
            NetworkManager.Instance.lastUsername = username;
            NetworkManager.Instance.SendRequest(request);
        }
        else
        {
            if (statusText != null)
            {
                statusText.text = "Error: Not connected to server.";
                statusText.color = Color.red;
            }
            if (loginButton != null)
            {
                loginButton.interactable = true;
            }
        }
    }

    private void OnLoginResponse(ExtendedEventArgs eventArgs)
    {
        ResponseLoginEventArgs args = eventArgs as ResponseLoginEventArgs;
        if (args != null)
        {
            if (loginButton != null)
            {
                loginButton.interactable = true;
            }

            if (args.Status == Constants.SUCCESS)
            {
                if (statusText != null)
                {
                    statusText.color = Color.green;
                    statusText.text = "Login successful!";
                }
                Debug.Log($"Logged in successfully! User ID: {args.PlayerId}, Username: {args.Username}");
                
                // Manually update the NetworkManager for silent reconnections
                NetworkManager.Instance.lastUsername = args.Username;
                NetworkManager.Instance.lastSessionToken = args.SessionToken;
                
                if (args.InGame) 
                {
                    Debug.Log("Rejoining active game in room: " + args.RoomId);
                    SceneManager.LoadScene("Reconnect"); // The LoL-style lock screen
                }
                else
                {
                    SceneManager.LoadScene("Lobby");
                }
            }
            else
            {
                if (statusText != null)
                {
                    statusText.color = Color.red;
                    statusText.text = "Login failed: " + args.Message;
                }
                Debug.LogWarning("Login failed: " + args.Message);
            }
        }
    }
}
