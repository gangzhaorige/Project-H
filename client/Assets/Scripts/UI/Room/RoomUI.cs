using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField createRoomInput;
    [SerializeField] private TMP_InputField joinRoomInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button refreshRoomButton;
    [SerializeField] private Button leaveRoomButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI currentRoomText;
    [SerializeField] private TextMeshProUGUI refreshRoomText;

    private void Start()
    {
        // Button Listeners
        if (createRoomButton != null) createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        if (joinRoomButton != null) joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        if (refreshRoomButton != null) refreshRoomButton.onClick.AddListener(OnRefreshRoomClicked);
        if (leaveRoomButton != null) leaveRoomButton.onClick.AddListener(OnLeaveRoomClicked);
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGameClicked);

        // Network Callbacks
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.AddCallback(Constants.SMSG_CREATE_ROOM, OnCreateRoomResponse);
            NetworkManager.Instance.AddCallback(Constants.SMSG_JOIN_ROOM, OnJoinRoomResponse);
            NetworkManager.Instance.AddCallback(Constants.SMSG_LEAVE_ROOM, OnLeaveRoomResponse);
            NetworkManager.Instance.AddCallback(Constants.SMSG_ALL_ROOMS, OnAllRoomsResponse);
            NetworkManager.Instance.AddCallback(Constants.SMSG_GAME_START, OnGameStartResponse);
        }
        
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_CREATE_ROOM);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_JOIN_ROOM);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_LEAVE_ROOM);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_ALL_ROOMS);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_GAME_START);
        }
    }

    private void UpdateUI()
    {
        bool inRoom = !string.IsNullOrEmpty(Constants.ROOM_ID);
        bool isHost = Constants.IS_HOST;

        if (createRoomInput != null) createRoomInput.gameObject.SetActive(!inRoom);
        if (joinRoomInput != null) joinRoomInput.gameObject.SetActive(!inRoom);
        
        if (createRoomButton != null) createRoomButton.gameObject.SetActive(!inRoom);
        if (joinRoomButton != null) joinRoomButton.gameObject.SetActive(!inRoom);
        if (refreshRoomButton != null) refreshRoomButton.gameObject.SetActive(!inRoom);
        
        if (leaveRoomButton != null) leaveRoomButton.gameObject.SetActive(inRoom);
        if (startGameButton != null) startGameButton.gameObject.SetActive(inRoom && isHost);

        if (currentRoomText != null)
        {
            currentRoomText.text = inRoom ? $"Current Room: {Constants.ROOM_NAME}" : "Not in a room";
        }
    }

    private void OnCreateRoomClicked()
    {
        string rName = createRoomInput != null ? createRoomInput.text : "";
        Debug.Log($"RoomUI: Requesting to CREATE room with name: '{rName}'");
        if (string.IsNullOrEmpty(rName)) return;

        RequestCreateRoom request = new RequestCreateRoom();
        request.Send(rName);
        NetworkManager.Instance.SendRequest(request);
    }

    private void OnJoinRoomClicked()
    {
        string rName = joinRoomInput != null ? joinRoomInput.text : "";
        Debug.Log($"RoomUI: Requesting to JOIN room with name: '{rName}'");
        if (string.IsNullOrEmpty(rName)) return;

        RequestJoinRoom request = new RequestJoinRoom();
        request.Send(rName);
        NetworkManager.Instance.SendRequest(request);
    }

    private void OnRefreshRoomClicked()
    {
        RequestAllRooms request = new RequestAllRooms();
        request.Send();
        NetworkManager.Instance.SendRequest(request);
    }

    private void OnLeaveRoomClicked()
    {
        RequestLeaveRoom request = new RequestLeaveRoom();
        request.Send();
        NetworkManager.Instance.SendRequest(request);
    }

    private void OnStartGameClicked()
    {
        RequestStartGame request = new RequestStartGame();
        request.Send();
        NetworkManager.Instance.SendRequest(request);
    }

    private void OnCreateRoomResponse(ExtendedEventArgs eventArgs)
    {
        ResponseCreateRoomEventArgs args = eventArgs as ResponseCreateRoomEventArgs;
        if (args != null && args.Status == Constants.SUCCESS)
        {
            Constants.ROOM_ID = args.RoomId;
            Constants.ROOM_NAME = args.RoomName;
            Constants.IS_HOST = true;
            UpdateUI();
        }
    }

    private void OnJoinRoomResponse(ExtendedEventArgs eventArgs)
    {
        ResponseJoinRoomEventArgs args = eventArgs as ResponseJoinRoomEventArgs;
        if (args != null && args.Status == Constants.SUCCESS)
        {
            Constants.ROOM_ID = args.RoomId;
            Constants.ROOM_NAME = args.RoomName;
            Constants.IS_HOST = false;
            UpdateUI();
        }
    }

    private void OnLeaveRoomResponse(ExtendedEventArgs eventArgs)
    {
        ResponseLeaveRoomEventArgs args = eventArgs as ResponseLeaveRoomEventArgs;
        if (args != null && args.Status == Constants.SUCCESS)
        {
            Constants.ROOM_ID = "";
            Constants.ROOM_NAME = "";
            UpdateUI();
        }
    }

    private void OnAllRoomsResponse(ExtendedEventArgs eventArgs)
    {
        ResponseAllRoomsEventArgs args = eventArgs as ResponseAllRoomsEventArgs;
        if (args != null && args.Status == Constants.SUCCESS)
        {
            if (refreshRoomText != null)
            {
                string roomList = $"Found {args.Rooms.Count} rooms:\n";
                foreach (var room in args.Rooms)
                {
                    roomList += $"{room.Name} ({room.PlayerCount} players)\n";
                }
                refreshRoomText.text = roomList;
            }

            Debug.Log($"Refreshed Room List: {args.Rooms.Count} rooms found.");
        }
    }

    private void OnGameStartResponse(ExtendedEventArgs eventArgs)
    {
        ResponseGameStartEventArgs args = eventArgs as ResponseGameStartEventArgs;
        if (args != null && args.Status == Constants.SUCCESS)
        {
            SceneManager.LoadScene("ChampSelect");
        }
    }
}
