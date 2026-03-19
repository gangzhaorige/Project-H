using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class GameplayManager : MonoBehaviour
{
    [Header("Components")]
    public ChampionSetup championSetup;

    void Start()
    {
        GameSession.Instance.Clear();

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

        // UI events are decoupled - UIController will listen to GameSession events

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
        GameSession.Instance.RequiredCardType = res.RequiredCardType;
        GameSession.Instance.IsResponseRequired = (res.PlayerId == Constants.USER_ID || isNegationWindow);

        Debug.Log($"[GameplayManager] Timer start for player {res.PlayerId}. seconds: {res.Seconds}, RequiredType: {res.RequiredCardType}");

        // Observer Pattern: Trigger event
        GameSession.Instance.TriggerTimerStarted(res.Seconds, res.Message, res.PlayerId);

        // Update Indicator for everyone (Model logic)
        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.ChampionObject != null)
            {
                var controller = player.ChampionObject.GetComponent<ChampionController>();
                if (controller != null)
                {
                    controller.ToggleActive(isNegationWindow || player.PlayerId == res.PlayerId);
                }
            }
        }
    }

    private void OnPassPriorityResponse(ExtendedEventArgs args)
    {
        ResponsePassPriorityEventArgs res = args as ResponsePassPriorityEventArgs;
        if (res == null) return;

        GameSession.Instance.IsResponseRequired = false;
        GameSession.Instance.RequiredCardType = "";
        
        GameSession.Instance.TriggerTimerCancelled();
    }

    private void OnTimerCancel(ExtendedEventArgs args)
    {
        GameSession.Instance.IsResponseRequired = false;
        GameSession.Instance.RequiredCardType = "";

        GameSession.Instance.TriggerTimerCancelled();
        
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
        if (res != null) CardManager.Instance.HandleLocalDraw(res.Cards);
    }

    private void OnCardDrawOther(ExtendedEventArgs args)
    {
        ResponseDrawCardOtherEventArgs res = args as ResponseDrawCardOtherEventArgs;
        if (res != null) CardManager.Instance.HandleOtherDraw(res.PlayerId, res.CardCount);
    }

    private void OnTurnStart(ExtendedEventArgs args)
    {
        ResponseTurnStartEventArgs res = args as ResponseTurnStartEventArgs;
        if (res == null) return;

        Debug.Log($"[GameplayManager] Turn started for player {res.ActivePlayerId}.");
        GameSession.Instance.ActivePlayerId = res.ActivePlayerId;
        GameSession.Instance.TriggerTurnStarted(res.ActivePlayerId);
    }

    private void OnTurnEnd(ExtendedEventArgs args)
    {
        ResponseEndTurnEventArgs res = args as ResponseEndTurnEventArgs;
        if (res != null) Debug.Log($"[GameplayManager] Turn ended for player {res.EndedPlayerId}");
        
        GameSession.Instance.ActivePlayerId = -1;
        GameSession.Instance.TriggerTimerCancelled();

        // Clear indicators
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
        ResponseGameSetupEventArgs res = args as ResponseGameSetupEventArgs;
        if (res == null) return;

        championSetup.InitializeChampions(res.Players);
        
        GameSession.Instance.TriggerGameSetupCompleted();

        // Handshake Step 2
        RequestReadyToPlay readyReq = new RequestReadyToPlay();
        readyReq.Send();
        NetworkManager.Instance.SendRequest(readyReq);
    }

    private void OnGameStateResponse(ExtendedEventArgs args) {
        ResponseGameStateEventArgs res = args as ResponseGameStateEventArgs;
        if (res != null && res.Status == Constants.SUCCESS) {
            Debug.Log("[GameplayManager] State Change: " + res.StateName);
            GameSession.Instance.State = res.StateName;
        }
    }
}
