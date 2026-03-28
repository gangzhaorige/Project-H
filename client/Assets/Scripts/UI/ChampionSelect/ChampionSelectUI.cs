using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ProjectH.Models;

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

    // Addressables Caching
    private Dictionary<int, ChampionSO> championSOCache = new Dictionary<int, ChampionSO>();
    private Dictionary<int, int> playerRequestedChampId = new Dictionary<int, int>();
    private List<AsyncOperationHandle> loadingHandles = new List<AsyncOperationHandle>();

    [System.Serializable]
    public class ChampionData
    {
        public int id;
        public string championName;
        public string path;
        public string element;
        public int additionalTargetForAttack;
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
                championNames[champ.id] = champ.championName;
                championInfo[champ.id] = champ;
            }
        }
    }

    private void PreloadChampion(int id)
    {
        GetChampionSO(id, (so) =>
        {
            // Preload icon
            if (so.champIcon != null && so.champIcon.RuntimeKeyIsValid())
            {
                var hIcon = Addressables.LoadAssetAsync<Sprite>(so.champIcon);
                loadingHandles.Add(hIcon);
            }
            // Preload select portrait
            if (so.champSelectImage != null && so.champSelectImage.RuntimeKeyIsValid())
            {
                var hSelect = Addressables.LoadAssetAsync<Sprite>(so.champSelectImage);
                loadingHandles.Add(hSelect);
            }
            // Preload path image
            if (so.pathImage != null && so.pathImage.RuntimeKeyIsValid())
            {
                var hPath = Addressables.LoadAssetAsync<Sprite>(so.pathImage);
                loadingHandles.Add(hPath);
            }
        });
    }

    private void GetChampionSO(int id, System.Action<ChampionSO> callback)
    {
        if (championSOCache.TryGetValue(id, out ChampionSO so))
        {
            callback?.Invoke(so);
            return;
        }

        string address = $"Data/Champions/{id}_champ_data";
        var handle = Addressables.LoadAssetAsync<ChampionSO>(address);
        loadingHandles.Add(handle);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                championSOCache[id] = op.Result;
                callback?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"[ChampionSelectUI] Failed to load ChampionSO for id {id} at address {address}");
            }
        };
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

        // Release Addressable handles
        foreach (var handle in loadingHandles)
        {
            if (handle.IsValid()) Addressables.Release(handle);
        }
        loadingHandles.Clear();
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

        // Selective Preloading: Only preload champions in the server's pool
        foreach (int champId in res.ChampionPool)
        {
            PreloadChampion(champId);
        }

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

            // Find image for icon
            Image img = null;
            Transform imageTransform = go.transform.Find("ChampionImage");
            if (imageTransform != null) img = imageTransform.GetComponent<Image>();
            if (img == null)
            {
                Image[] images = go.GetComponentsInChildren<Image>();
                foreach (var i in images)
                {
                    if (i.gameObject != go) { img = i; break; }
                }
            }

            if (img != null)
            {
                LoadPoolIcon(champId, img);
            }
            
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChampionClick(champId));
            championButtons[champId] = btn;
        }

        UpdateInteractivity();
    }

    private void LoadPoolIcon(int champId, Image img)
    {
        GetChampionSO(champId, (so) =>
        {
            if (so.champIcon == null || !so.champIcon.RuntimeKeyIsValid()) return;
            var handle = Addressables.LoadAssetAsync<Sprite>(so.champIcon);
            loadingHandles.Add(handle);
            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded && img != null)
                {
                    img.sprite = op.Result;
                }
            };
        });
    }

    private void UpdatePlayerPanel(int playerId, int championId, bool lockedIn)
    {
        if (!playerPanels.ContainsKey(playerId)) return;
        playerRequestedChampId[playerId] = championId;

        GetChampionSO(championId, (so) =>
        {
            if (playerRequestedChampId[playerId] != championId) return;

            string displayName = $"{so.championName} ({so.path}/{so.element})";
            string elementStr = so.element.ToString();

            // Load Select Image if valid
            if (so.champSelectImage != null && so.champSelectImage.RuntimeKeyIsValid())
            {
                var portraitHandle = Addressables.LoadAssetAsync<Sprite>(so.champSelectImage);
                loadingHandles.Add(portraitHandle);
                portraitHandle.Completed += (pOp) =>
                {
                    if (pOp.Status != AsyncOperationStatus.Succeeded || playerRequestedChampId[playerId] != championId) return;

                    Sprite portrait = pOp.Result;
                    
                    // Load Path Image if valid
                    if (so.pathImage != null && so.pathImage.RuntimeKeyIsValid())
                    {
                        var pathHandle = Addressables.LoadAssetAsync<Sprite>(so.pathImage);
                        loadingHandles.Add(pathHandle);
                        pathHandle.Completed += (pathOp) =>
                        {
                            if (pathOp.Status != AsyncOperationStatus.Succeeded || playerRequestedChampId[playerId] != championId) return;

                            playerPanels[playerId].UpdateChampion(displayName, portrait, pathOp.Result, elementStr);
                            if (lockedIn) playerPanels[playerId].SetLockedIn(true);
                        };
                    }
                    else
                    {
                        playerPanels[playerId].UpdateChampion(displayName, portrait, null, elementStr);
                        if (lockedIn) playerPanels[playerId].SetLockedIn(true);
                    }
                };
            }
            else
            {
                playerPanels[playerId].UpdateChampion(displayName, null, null, elementStr);
                if (lockedIn) playerPanels[playerId].SetLockedIn(true);
            }
        });
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
        selectedChampionId = champId;
        confirmButton.interactable = isMyTurn;

        if (!isMyTurn) return;

        // Send hover request
        RequestSelectChampion hoverReq = new RequestSelectChampion();
        hoverReq.Send(champId);
        NetworkManager.Instance.SendRequest(hoverReq);
    }

    private void OnConfirmClick()
    {
        if (selectedChampionId == -1 || !isMyTurn) return;

        RequestPickChampion pickReq = new RequestPickChampion();
        pickReq.Send(selectedChampionId);
        NetworkManager.Instance.SendRequest(pickReq);
        
        isMyTurn = false;
        confirmButton.interactable = false;
        UpdateInteractivity();
    }

    private void OnPlayerHover(ExtendedEventArgs args)
    {
        ResponseNotifyPlayerSelectEventArgs res = args as ResponseNotifyPlayerSelectEventArgs;
        UpdatePlayerPanel(res.PlayerId, res.ChampionId, false);
    }

    private void OnPlayerPick(ExtendedEventArgs args)
    {
        ResponseNotifyPlayerPickEventArgs res = args as ResponseNotifyPlayerPickEventArgs;
        pickedChampionIds.Add(res.ChampionId);
        playersWhoPicked.Add(res.PlayerId);

        UpdatePlayerPanel(res.PlayerId, res.ChampionId, true);
        UpdateInteractivity();
    }

    private void OnSelectionCompleted(ExtendedEventArgs args)
    {
        isTimerActive = false;
        if (timerText != null) timerText.text = "0";
        statusText.text = "Champion Selection Completed! Loading game...";
        confirmButton.gameObject.SetActive(false);
        Invoke("LoadGameScene", 2.0f);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    private void UpdateInteractivity()
    {
        foreach (var pair in championButtons)
        {
            if (pair.Value != null)
            {
                bool isPicked = pickedChampionIds.Contains(pair.Key);
                pair.Value.interactable = !isPicked;
                if (isPicked) pair.Value.GetComponent<Image>().color = Color.gray;
            }
        }
        
        if (selectedChampionId != -1)
        {
            bool isPicked = pickedChampionIds.Contains(selectedChampionId);
            confirmButton.interactable = isMyTurn && !isPicked;
        }
    }
}
