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
    public TextMeshProUGUI turnStatusText;

    void Start()
    {
        GameSession.Instance.Clear();
        if (loadingPanel != null) loadingPanel.SetActive(true);

        if (championSetup == null) {
            Debug.LogError("[GameplayManager] CRITICAL: championSetup reference is null in Start!");
        }

        Debug.Log("[GameplayManager] Registering SMSG_GAME_SETUP callback.");
        NetworkManager.Instance.AddCallback(Constants.SMSG_GAME_SETUP, OnGameSetup);
        NetworkManager.Instance.AddCallback(Constants.SMSG_CARD_DRAW, OnCardDraw);
        NetworkManager.Instance.AddCallback(Constants.SMSG_CARD_DRAW_OTHER, OnCardDrawOther);
        NetworkManager.Instance.AddCallback(Constants.SMSG_TURN_START, OnTurnStart);
        NetworkManager.Instance.AddCallback(Constants.SMSG_END_TURN, OnTurnEnd);
        
        if (endTurnButton != null) {
            endTurnButton.onClick.AddListener(OnEndTurnClick);
            endTurnButton.interactable = false;
        }

        // Handshake Step 1: Tell server we loaded the scene and are ready for data
        Debug.Log("[GameplayManager] Sending RequestReadyForGameSetup...");
        RequestReadyForGameSetup setupReq = new RequestReadyForGameSetup();
        setupReq.Send();
        NetworkManager.Instance.SendRequest(setupReq);
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
}
