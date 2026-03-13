using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampionSelectUI : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject championPrefab; // Button with TextMeshPro
    public GameObject playerInfoPrefab; // Prefab with ChampionPanel attached

    [Header("Containers")]
    public Transform blueTeamContainer;
    public Transform redTeamContainer;
    public Transform championPoolContainer;

    [Header("Controls")]
    public Button confirmButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timerText;

    private Dictionary<int, string> championNames = new Dictionary<int, string>();
    private Dictionary<int, ChampionPanel> playerPanels = new Dictionary<int, ChampionPanel>();
    private Dictionary<int, Button> championButtons = new Dictionary<int, Button>();
    private HashSet<int> pickedChampionIds = new HashSet<int>();
    private HashSet<int> playersWhoPicked = new HashSet<int>();

    private int selectedChampionId = -1;
    private bool isMyTurn = false;
    private int activePlayerId = -1;
    private float turnTimer = 0f;
    private bool isTimerActive = false;

    [System.Serializable]
    public class ChampionData
    {
        public int id;
        public string name;
        public int pathId;
        public string element;
    }

    [System.Serializable]
    public class ChampionList
    {
        public List<ChampionData> champions;
    }

    void Start()
    {
        LoadChampionData();
        NetworkManager.Instance.AddCallback(Constants.SMSG_CHAMPION_SELECT_READY, OnChampionSelectReady);
        NetworkManager.Instance.AddCallback(Constants.SMSG_NOTIFY_FOR_CHAMPION_PICK, OnNotifyTurn);
        NetworkManager.Instance.AddCallback(Constants.SMSG_NOTIFY_PLAYER_SELECT, OnPlayerHover);
        NetworkManager.Instance.AddCallback(Constants.SMSG_NOTIFY_PLAYER_PICK, OnPlayerPick);
        NetworkManager.Instance.AddCallback(Constants.SMSG_CHAMPION_SELECT_COMPLETED, OnSelectionCompleted);

        confirmButton.onClick.AddListener(OnConfirmClick);
        confirmButton.interactable = false;
        
        // Let the server know we are ready to receive data
        RequestReadyForChampionSelect readyReq = new RequestReadyForChampionSelect();
        readyReq.Send();
        NetworkManager.Instance.SendRequest(readyReq);
    }

    void Update()
    {
        if (isTimerActive)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                turnTimer = 0;
                isTimerActive = false;
            }
            UpdateStatusText();
        }
    }

    private void UpdateStatusText()
    {
        statusText.text = isMyTurn ? "Your turn to pick!" : $"Player {activePlayerId} is picking...";
        if (timerText != null)
        {
            timerText.text = $"{Mathf.CeilToInt(turnTimer)}";
        }
    }

    private Dictionary<int, ChampionData> championInfo = new Dictionary<int, ChampionData>();

    private void LoadChampionData()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/champions");
        if (jsonText != null)
        {
            string fixedJson = "{\"champions\":" + jsonText.text + "}";
            ChampionList data = JsonUtility.FromJson<ChampionList>(fixedJson);
            foreach (var champ in data.champions)
            {
                championNames[champ.id] = champ.name;
                championInfo[champ.id] = champ;
            }
        }
    }

    private Sprite GetChampionIcon(int championId)
    {
        Sprite s = Resources.Load<Sprite>($"Images/Characters/Icons/{championId}");
        if (s == null) s = Resources.Load<Sprite>("Images/Characters/Icons/default");
        return s;
    }

    private Sprite GetChampionPortrait(int championId)
    {
        Sprite s = Resources.Load<Sprite>($"Images/Characters/Portraits/{championId}");
        if (s == null) s = Resources.Load<Sprite>("Images/Characters/Portraits/default");
        return s;
    }

    private Sprite GetPathIcon(int pathId)
    {
        return Resources.Load<Sprite>($"Images/Paths/{pathId}");
    }

    void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_CHAMPION_SELECT_READY);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_NOTIFY_FOR_CHAMPION_PICK);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_NOTIFY_PLAYER_SELECT);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_NOTIFY_PLAYER_PICK);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_CHAMPION_SELECT_COMPLETED);
        }
    }

    private void OnChampionSelectReady(ExtendedEventArgs args)
    {
        ResponseChampionSelectReadyEventArgs res = args as ResponseChampionSelectReadyEventArgs;
        
        // Clear containers
        foreach (Transform child in blueTeamContainer) Destroy(child.gameObject);
        foreach (Transform child in redTeamContainer) Destroy(child.gameObject);
        foreach (Transform child in championPoolContainer) Destroy(child.gameObject);
        playerPanels.Clear();
        championButtons.Clear();
        pickedChampionIds.Clear();
        playersWhoPicked.Clear();

        // Create player displays
        foreach (var player in res.Players)
        {
            Transform teamContainer = (player.Team == Constants.TEAM_BLUE) ? blueTeamContainer : redTeamContainer;
            GameObject go = Instantiate(playerInfoPrefab, teamContainer, false);
            ChampionPanel panel = go.GetComponent<ChampionPanel>();
            if (panel == null) panel = go.AddComponent<ChampionPanel>();

            panel.Init(player.PlayerId, player.Username, player.Team);
            playerPanels[player.PlayerId] = panel;
        }

        // Create champion pool
        foreach (int champId in res.ChampionPool)
        {
            GameObject go = Instantiate(championPrefab, championPoolContainer);
            TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
            string champName = championNames.ContainsKey(champId) ? championNames[champId] : "Unknown " + champId;
            if (txt != null) txt.text = champName;

            // Try to find ChampionImage child
            Transform imageTransform = go.transform.Find("ChampionImage");
            Image img = (imageTransform != null) ? imageTransform.GetComponent<Image>() : null;
            
            // If not found by name, try to find the first image that is not the button background
            if (img == null)
            {
                Image[] images = go.GetComponentsInChildren<Image>();
                foreach (var i in images)
                {
                    if (i.gameObject != go) // Avoid the button background itself if it's on the root
                    {
                        img = i;
                        break;
                    }
                }
            }

            if (img != null)
            {
                img.sprite = GetChampionIcon(champId);
            }
            
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChampionClick(champId));
            championButtons[champId] = btn;
        }

        UpdateInteractivity();
    }

    private void OnNotifyTurn(ExtendedEventArgs args)
    {
        ResponseNotifyForChampionPickEventArgs res = args as ResponseNotifyForChampionPickEventArgs;
        activePlayerId = res.ActivePlayerId;
        isMyTurn = (activePlayerId == Constants.USER_ID);

        turnTimer = res.Timeout;
        isTimerActive = true;

        UpdateStatusText();

        // Update player panels statuses
        foreach (var pair in playerPanels)
        {
            if (!playersWhoPicked.Contains(pair.Key))
            {
                string status = (pair.Key == activePlayerId) ? "Picking..." : "Waiting...";
                pair.Value.UpdateChampion(status);
            }
        }

        UpdateInteractivity();
    }
    private void OnChampionClick(int champId)
    {
        Debug.Log($"OnChampionClick: champId={champId}, isMyTurn={isMyTurn}");
        
        selectedChampionId = champId;
        confirmButton.interactable = isMyTurn;

        if (!isMyTurn)
        {
            Debug.Log("Not my turn, not sending hover request.");
            return;
        }

        // Send hover request
        RequestSelectChampion hoverReq = new RequestSelectChampion();
        hoverReq.Send(champId);
        NetworkManager.Instance.SendRequest(hoverReq);
    }

    private void OnConfirmClick()
    {
        Debug.Log($"OnConfirmClick: selectedChampionId={selectedChampionId}, isMyTurn={isMyTurn}");
        if (selectedChampionId == -1 || !isMyTurn) return;

        RequestPickChampion pickReq = new RequestPickChampion();
        pickReq.Send(selectedChampionId);
        NetworkManager.Instance.SendRequest(pickReq);
        
        isMyTurn = false;
        confirmButton.interactable = false;
        UpdateInteractivity();
    }

    private string GetPathName(int pathId)
    {
        switch (pathId)
        {
            case 0: return "Destruction";
            case 1: return "Hunt";
            case 2: return "Erudition";
            case 3: return "Harmony";
            case 4: return "Nihility";
            case 5: return "Preservation";
            case 6: return "Abundance";
            case 7: return "Remembrance";
            default: return "Unknown";
        }
    }

    private void OnPlayerHover(ExtendedEventArgs args)
    {
        ResponseNotifyPlayerSelectEventArgs res = args as ResponseNotifyPlayerSelectEventArgs;
        if (playerPanels.ContainsKey(res.PlayerId))
        {
            // Update image/text
            ChampionData info = championInfo.ContainsKey(res.ChampionId) ? championInfo[res.ChampionId] : null;
            string displayName = info != null ? $"{info.name} ({GetPathName(info.pathId)}/{info.element})" : "Picking...";
            Sprite sprite = GetChampionPortrait(res.ChampionId);
            Sprite pathSprite = info != null ? GetPathIcon(info.pathId) : null;

            playerPanels[res.PlayerId].UpdateChampion(displayName, sprite, pathSprite, info != null ? info.element : "");
            Debug.Log($"Player {res.PlayerId} hovered champion {res.ChampionId}");
        }
    }
private void OnPlayerPick(ExtendedEventArgs args)
{
    ResponseNotifyPlayerPickEventArgs res = args as ResponseNotifyPlayerPickEventArgs;
    pickedChampionIds.Add(res.ChampionId);
    playersWhoPicked.Add(res.PlayerId);

    if (playerPanels.ContainsKey(res.PlayerId))
        {
            // Lock in image/text
            ChampionData info = championInfo.ContainsKey(res.ChampionId) ? championInfo[res.ChampionId] : null;
            string displayName = info != null ? $"{info.name} ({GetPathName(info.pathId)}/{info.element})" : "Picking...";
            Sprite sprite = GetChampionPortrait(res.ChampionId);
            Sprite pathSprite = info != null ? GetPathIcon(info.pathId) : null;

            playerPanels[res.PlayerId].UpdateChampion(displayName, sprite, pathSprite, info != null ? info.element : "");
            playerPanels[res.PlayerId].SetLockedIn(true);
            Debug.Log($"Player {res.PlayerId} locked in champion {res.ChampionId}");
        }

        UpdateInteractivity();
    }
    private void OnSelectionCompleted(ExtendedEventArgs args)
    {
        isTimerActive = false;
        if (timerText != null)
        {
            timerText.text = "0";
        }
        statusText.text = "Champion Selection Completed!";
        confirmButton.gameObject.SetActive(false);
    }

    private void UpdateInteractivity()
    {
        foreach (var pair in championButtons)
        {
            if (pair.Value != null)
            {
                bool isPicked = pickedChampionIds.Contains(pair.Key);
                pair.Value.interactable = !isPicked;
                
                if (isPicked)
                {
                    pair.Value.GetComponent<Image>().color = Color.gray;
                }
            }
        }
        
        if (selectedChampionId != -1)
        {
            bool isPicked = pickedChampionIds.Contains(selectedChampionId);
            confirmButton.interactable = isMyTurn && !isPicked;
        }
    }
}
