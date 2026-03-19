using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ProjectH.Models;

public class GameplayManager : MonoBehaviour
{
    [Header("UI - Loading")]
    public GameObject loadingPanel;

    [Header("Components")]
    public ChampionSetup championSetup;

    [Header("Turn UI")]
    public Button endTurnButton;
    public Button passPriorityButton;
    public TextMeshProUGUI turnStatusText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI stateText;
    public Slider timerSlider;

    private float currentTimer = 0f;
    private float maxTimer = 0f;
    private bool isTimerActive = false;


    void Start()
    {
        GameSession.Instance.Clear();
        if (loadingPanel != null) loadingPanel.SetActive(true);

        if (championSetup == null) {
            Debug.LogError("[GameplayManager] CRITICAL: championSetup reference is null in Start!");
        }

        Debug.Log("[GameplayManager] Registering callbacks.");
        NetworkManager.Instance.AddCallback(Constants.SMSG_GAME_SETUP, OnGameSetup);
        NetworkManager.Instance.AddCallback(Constants.SMSG_CARD_DRAW, OnCardDraw);
        NetworkManager.Instance.AddCallback(Constants.SMSG_CARD_DRAW_OTHER, OnCardDrawOther);
        NetworkManager.Instance.AddCallback(Constants.SMSG_TURN_START, OnTurnStart);
        NetworkManager.Instance.AddCallback(Constants.SMSG_END_TURN, OnTurnEnd);
        NetworkManager.Instance.AddCallback(Constants.SMSG_RESPONSE_TIMER_START, OnTimerStart);
        NetworkManager.Instance.AddCallback(Constants.SMSG_RESPONSE_TIMER_CANCEL, OnTimerCancel);
        NetworkManager.Instance.AddCallback(Constants.SMSG_PASS_PRIORITY, OnPassPriorityResponse);
        NetworkManager.Instance.AddCallback(Constants.SMSG_STATE_CHANGE, OnGameStateResponse);

        if (timerSlider != null) timerSlider.gameObject.SetActive(false);
        
        if (endTurnButton != null) {
            endTurnButton.onClick.AddListener(OnEndTurnClick);
            endTurnButton.interactable = false;
        }

        if (passPriorityButton != null) {
            passPriorityButton.onClick.AddListener(OnPassPriorityClick);
            passPriorityButton.gameObject.SetActive(false);
        }

        // Handshake Step 1: Tell server we loaded the scene and are ready for data
        Debug.Log("[GameplayManager] Sending RequestReadyForGameSetup...");
        RequestReadyForGameSetup setupReq = new RequestReadyForGameSetup();
        setupReq.Send();
        NetworkManager.Instance.SendRequest(setupReq);
    }

    void Update()
    {
        if (isTimerActive && currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            if (timerSlider != null)
            {
                timerSlider.value = currentTimer / maxTimer;
            }
        }
    }
    

    void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_GAME_SETUP);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_CARD_DRAW);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_CARD_DRAW_OTHER);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_TURN_START);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_END_TURN);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_RESPONSE_TIMER_START);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_RESPONSE_TIMER_CANCEL);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_PASS_PRIORITY);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_STATE_CHANGE);
        }
    }

    private void OnTimerStart(ExtendedEventArgs args)
    {
        ResponseTimerStartEventArgs res = args as ResponseTimerStartEventArgs;
        if (res == null) return;

        bool isNegationWindow = (res.PlayerId == -1);
        bool isLocal = res.PlayerId == Constants.USER_ID;
        Debug.Log($"[GameplayManager] Timer start for player {res.PlayerId}. isLocal: {isLocal}, isNegationWindow: {isNegationWindow}, seconds: {res.Seconds}");
        
        // Update Indicator for everyone
        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.ChampionObject != null)
            {
                var controller = player.ChampionObject.GetComponent<ChampionController>();
                if (controller != null)
                {
                    // If it's a negation window, everyone pulses. Otherwise only the active player.
                    controller.ToggleActive(isNegationWindow || player.PlayerId == res.PlayerId);
                }
            }
        }

        // Show Pass button during negation window
        if (passPriorityButton != null)
        {
            passPriorityButton.gameObject.SetActive(isNegationWindow || (isLocal && GameSession.Instance.State != "PlayActionState"));
        }

        // Display instructional message
        if (instructionText != null)
        {
            instructionText.text = res.Message;
        }

        // Update Slider for local player or everyone during negation
        if ((isLocal || isNegationWindow) && timerSlider != null)
        {
            timerSlider.gameObject.SetActive(true);
            timerSlider.interactable = false; // Display only
            currentTimer = res.Seconds;
            maxTimer = (res.Seconds > 0) ? res.Seconds : 15f;
            isTimerActive = true;
        }
        else if (timerSlider != null)
        {
            timerSlider.gameObject.SetActive(false);
            isTimerActive = false;
        }
    }

    private void OnPassPriorityClick()
    {
        Debug.Log("[GameplayManager] Sending RequestPassPriority...");
        RequestPassPriority req = new RequestPassPriority();
        req.Send();
        NetworkManager.Instance.SendRequest(req);
    }

    private void OnPassPriorityResponse(ExtendedEventArgs args)
    {
        ResponsePassPriorityEventArgs res = args as ResponsePassPriorityEventArgs;
        if (res == null) return;

        if (passPriorityButton != null) passPriorityButton.gameObject.SetActive(false);
        if (timerSlider != null) timerSlider.gameObject.SetActive(false);
        isTimerActive = false;
    }

    private void OnTimerCancel(ExtendedEventArgs args)
    {
        isTimerActive = false;
        if (timerSlider != null) timerSlider.gameObject.SetActive(false);
        if (passPriorityButton != null) passPriorityButton.gameObject.SetActive(false);
        if (instructionText != null) instructionText.text = "";

        // Clear all pulsing indicators
        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.ChampionObject != null)
            {
                var controller = player.ChampionObject.GetComponent<ChampionController>();
                if (controller != null) controller.ToggleActive(false);
            }
        }
    }

    private void OnCardDraw(ExtendedEventArgs args)
    {
        ResponseDrawCardEventArgs res = args as ResponseDrawCardEventArgs;
        if (res != null)
        {
            CardManager.Instance.HandleLocalDraw(res.Cards);
        }
    }

    private void OnCardDrawOther(ExtendedEventArgs args)
    {
        ResponseDrawCardOtherEventArgs res = args as ResponseDrawCardOtherEventArgs;
        if (res != null)
        {
            CardManager.Instance.HandleOtherDraw(res.PlayerId, res.CardCount);
        }
    }

    private void OnEndTurnClick()
    {
        Debug.Log("[GameplayManager] Sending RequestEndTurn...");
        RequestEndTurn req = new RequestEndTurn();
        req.Send();
        NetworkManager.Instance.SendRequest(req);
        
        if (endTurnButton != null) endTurnButton.interactable = false;
    }

    private void OnTurnStart(ExtendedEventArgs args)
    {
        ResponseTurnStartEventArgs res = args as ResponseTurnStartEventArgs;
        if (res == null) return;

        bool isMyTurn = (res.ActivePlayerId == Constants.USER_ID);
        Debug.Log($"[GameplayManager] Turn started for player {res.ActivePlayerId}. My turn: {isMyTurn}");
        
        if (turnStatusText != null) {
            turnStatusText.text = isMyTurn ? "YOUR TURN" : $"PLAYER {res.ActivePlayerId} TURN";
        }

        if (endTurnButton != null) {
            endTurnButton.interactable = isMyTurn;
        }
    }

    private void OnTurnEnd(ExtendedEventArgs args)
    {
        ResponseEndTurnEventArgs res = args as ResponseEndTurnEventArgs;
        if (res != null) {
            Debug.Log($"[GameplayManager] Turn ended for player {res.EndedPlayerId}");
        }
        
        isTimerActive = false;
        if (timerSlider != null) timerSlider.gameObject.SetActive(false);
        if (passPriorityButton != null) passPriorityButton.gameObject.SetActive(false);

        // Clear all pulsing indicators
        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.ChampionObject != null)
            {
                var controller = player.ChampionObject.GetComponent<ChampionController>();
                if (controller != null) controller.ToggleActive(false);
            }
        }
    }

    private void OnGameSetup(ExtendedEventArgs args)
    {
        Debug.Log("OnGameSetup callback received.");
        ResponseGameSetupEventArgs res = args as ResponseGameSetupEventArgs;
        
        if (res == null) {
            Debug.LogError("Failed to cast args to ResponseGameSetupEventArgs");
            return;
        }

        if (championSetup == null) {
            Debug.LogError("championSetup reference is missing in GameplayManager!");
            return;
        }

        Debug.Log($"Starting initialization for {res.Players.Count} players.");
        championSetup.InitializeChampions(res.Players);

        FinishInitialization();
    }

    private void FinishInitialization()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);

        // Handshake Step 2: Tell server we finished reconstruction and are ready to play
        RequestReadyToPlay readyReq = new RequestReadyToPlay();
        readyReq.Send();
        NetworkManager.Instance.SendRequest(readyReq);
        
        Debug.Log("Game Initialization Finished. ReadyToPlay sent.");
    }

    private void OnGameStateResponse(ExtendedEventArgs args) {
        ResponseGameStateEventArgs res = args as ResponseGameStateEventArgs;
        if (res != null && res.Status == Constants.SUCCESS) {
            Debug.Log("Switching to state: " + res.StateName);
            if (stateText != null) {
                stateText.text = res.StateName;
            }
            GameSession.Instance.State = res.StateName;
        }
    }
}
