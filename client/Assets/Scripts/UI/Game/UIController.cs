using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectH.Models;

public class UIController : MonoBehaviour
{
    private Button endTurnButton;
    private Button passButton;
    private Button confirmPlayButton; // New button for playing selected cards
    
    private TextMeshProUGUI turnStatusText;
    private TextMeshProUGUI instructionText;
    private TextMeshProUGUI stateText;
    
    private GameObject timerUI;
    private Slider timerSlider;

    private ProjectH.UI.TargetSelectionPanelView targetSelectionPanelView;
    private GameObject selectPanel;

    private float currentTimer = 0f;
    private float maxTimer = 0f;
    private bool isTimerActive = false;

    private HandManager _handManager;
    private CardTargetSelector _cardTargetSelector;

    public void Init(HandManager handManager, CardTargetSelector cardTargetSelector, ProjectH.UI.CanvasView canvasView)
    {
        _handManager = handManager;
        _cardTargetSelector = cardTargetSelector;

        if (canvasView != null)
        {
            endTurnButton = canvasView.endTurnButton;
            passButton = canvasView.passButton;
            confirmPlayButton = canvasView.confirmPlayButton;
            turnStatusText = canvasView.turnStatusText;
            instructionText = canvasView.instructionText;
            stateText = canvasView.stateText;
            timerUI = canvasView.timerUI;
            timerSlider = canvasView.timerSlider;
            targetSelectionPanelView = canvasView.targetSelectionPanelView;
            selectPanel = canvasView.selectPanelView != null ? canvasView.selectPanelView.gameObject : null;

            // Add button listeners here instead of OnEnable if we want strict DI control
            if (endTurnButton != null) endTurnButton.onClick.AddListener(OnEndTurnClick);
            if (passButton != null) passButton.onClick.AddListener(OnPassPriorityClick);
            if (confirmPlayButton != null) confirmPlayButton.onClick.AddListener(OnConfirmPlayClick);
        }
        
        if (_handManager != null)
        {
            _handManager.OnSelectionChanged += UpdateUIState;
        }

        HideAll();
    }

    private void OnEnable()
    {
        // Subscribe to GameSession events (Observer Pattern)
        GameSession.Instance.OnStateChanged += OnGameStateChanged;
        GameSession.Instance.OnResponseRequirementChanged += OnResponseRequirementChanged;
        GameSession.Instance.OnTimerStarted += OnTimerStarted;
        GameSession.Instance.OnTimerCancelled += OnTimerCancelled;
        GameSession.Instance.OnTurnStarted += OnTurnStarted;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        if (GameSession.Instance != null)
        {
            GameSession.Instance.OnStateChanged -= OnGameStateChanged;
            GameSession.Instance.OnResponseRequirementChanged -= OnResponseRequirementChanged;
            GameSession.Instance.OnTimerStarted -= OnTimerStarted;
            GameSession.Instance.OnTimerCancelled -= OnTimerCancelled;
            GameSession.Instance.OnTurnStarted -= OnTurnStarted;
        }

        if (_handManager != null)
        {
            _handManager.OnSelectionChanged -= UpdateUIState;
        }

        // Clean up listeners
        if (endTurnButton != null) endTurnButton.onClick.RemoveListener(OnEndTurnClick);
        if (passButton != null) passButton.onClick.RemoveListener(OnPassPriorityClick);
        if (confirmPlayButton != null) confirmPlayButton.onClick.RemoveListener(OnConfirmPlayClick);
    }

    private void Update()
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

    // --- Observer Event Handlers ---

    private void OnGameStateChanged(string newState)
    {
        SetGameStateText(newState);
        UpdateUIState();
    }

    private void OnResponseRequirementChanged(bool isRequired)
    {
        UpdateUIState();
    }

    private void OnTimerStarted(int seconds, string message, int playerId)
    {
        SetInstruction(message);
        bool isNegationWindow = (playerId == -1);
        bool isLocal = (playerId == Constants.USER_ID);

        if (isLocal || isNegationWindow)
        {
            StartTimer(seconds);
        }
        else
        {
            StopTimer();
        }
        UpdateUIState();
    }

    private void OnTimerCancelled()
    {
        StopTimer();
        SetInstruction("");
        UpdateUIState();
    }

    private void OnTurnStarted(int activePlayerId)
    {
        bool isMyTurn = (activePlayerId == Constants.USER_ID);
        SetTurnStatus(isMyTurn ? "YOUR TURN" : $"PLAYER {activePlayerId} TURN");
        
        if (endTurnButton != null) endTurnButton.interactable = isMyTurn;
        UpdateUIState();
    }

    // --- Action Handlers ---

    private void OnEndTurnClick()
    {
        Debug.Log("[UIController] Sending RequestEndTurn...");
        RequestEndTurn req = new RequestEndTurn();
        req.Send();
        NetworkManager.Instance.SendRequest(req);
        
        if (endTurnButton != null) endTurnButton.interactable = false;
    }

    private void OnPassPriorityClick()
    {
        Debug.Log("[UIController] Sending RequestPassPriority...");
        RequestPassPriority req = new RequestPassPriority();
        req.Send();
        NetworkManager.Instance.SendRequest(req);
    }

    // --- UI Utility Methods ---

    public void SetTurnStatus(string status)
    {
        if (turnStatusText != null) turnStatusText.text = status;
    }

    public void SetInstruction(string instruction)
    {
        if (instructionText != null) instructionText.text = instruction;
    }

    public void SetGameStateText(string stateName)
    {
        if (stateText != null) stateText.text = stateName;
    }

    public void StartTimer(float seconds)
    {
        currentTimer = seconds;
        maxTimer = (seconds > 0) ? seconds : 15f;
        isTimerActive = true;
        if (timerSlider != null) {
            timerSlider.value = 1f;
            timerSlider.gameObject.SetActive(true);
        }
        ShowTimerUI(true);
    }

    public void StopTimer()
    {
        isTimerActive = false;
        if (timerSlider != null) timerSlider.gameObject.SetActive(false);
        ShowTimerUI(false);
    }

    public void ShowEndTurnButton(bool show)
    {
        if (endTurnButton != null) endTurnButton.gameObject.SetActive(show);
    }

    public void ShowPassButton(bool show)
    {
        if (passButton != null) passButton.gameObject.SetActive(show);
    }

    public void ShowTimerUI(bool show)
    {
        if (timerUI != null) timerUI.SetActive(show);
    }

    public void ShowConfirmPlayButton(bool show)
    {
        if (confirmPlayButton != null) confirmPlayButton.gameObject.SetActive(show);
    }

    public void ShowTargetSelectionPanel(bool show)
    {
        if (targetSelectionPanelView != null) targetSelectionPanelView.gameObject.SetActive(show);
    }

    public void ShowSelectPanel(bool show)
    {
        if (selectPanel != null) selectPanel.SetActive(show);
    }

    public void HideAll()
    {
        ShowEndTurnButton(false);
        ShowPassButton(false);
        ShowConfirmPlayButton(false);
        ShowTimerUI(false);
        ShowTargetSelectionPanel(false);
        ShowSelectPanel(false);
        SetInstruction("");
    }

    public void UpdateUIState()
    {
        string currentState = GameSession.Instance.State;
        bool isResponseRequired = GameSession.Instance.IsResponseRequired;
        bool isLocalTurn = GameSession.Instance.IsLocalTurn;

        // A "forced" response is one where a specific card type is required (e.g. Duel, Defend, Negate)
        // In PlayActionState, the "ANY" timer is NOT a forced response phase.
        bool isForcedResponse = isResponseRequired && (GameSession.Instance.RequiredCardType != -1);

        // 1. End Turn Button: Only visible during local player's PlayActionState if no forced response is pending
        bool canEndTurn = (currentState == "PlayActionState" && isLocalTurn && !isForcedResponse);
        ShowEndTurnButton(canEndTurn);

        // 2. Pass Button: Only visible when a forced response is required (Duel, Defend, or Negation window)
        bool canPass = isForcedResponse;
        ShowPassButton(canPass);

        // 3. Confirm Play Button: Visible when at least one card is selected
        bool hasSelection = (_handManager != null && _handManager.GetSelectedCardIds().Count > 0);
        ShowConfirmPlayButton(hasSelection);

        if (confirmPlayButton != null)
        {
            bool multipleCards = _handManager != null && _handManager.GetSelectedCardIds().Count > 1;
            if (currentState == "PlayActionState" && !isForcedResponse && multipleCards)
            {
                confirmPlayButton.interactable = false;
            }
            else
            {
                confirmPlayButton.interactable = true;
            }
        }
    }

    private void OnConfirmPlayClick()
    {
        if (_handManager == null) return;

        System.Collections.Generic.List<int> selectedIds = _handManager.GetSelectedCardIds();
        if (selectedIds.Count == 0) return;

        // Currently, we only process the first selected card for playing/targeting.
        // In the future, multiple cards can be sent together for specific skills.
        int cardId = selectedIds[0];
        CardData cardData = GameSession.Instance.GetLocalPlayer().Hand.Find(c => c.Id == cardId);
        
        if (cardData == null) return;

        if (GameSession.Instance.State == "SkillResolutionState")
        {
            SendPlayRequest(selectedIds, new System.Collections.Generic.List<int>());
            return;
        }

        if (GameSession.Instance.State == "PlayActionState")
        {
            int maxTargets = ProjectH.Rules.CardRuleManager.GetMaxTargets(cardData);
            if (maxTargets > 0)
            {
                Debug.Log($"[UIController] Card {cardData.Type} requires {maxTargets} targets. Showing UI.");
                if (_cardTargetSelector != null) _cardTargetSelector.BeginTargeting(cardData); // Wait, we might need selectedIds? Currently CardTargetSelector only handles 1 card.
            }
            else
            {
                Debug.Log($"[UIController] Card {cardData.Type} requires no targets. Playing directly.");
                SendPlayRequest(selectedIds, new System.Collections.Generic.List<int>());
            }
            return;
        }

        if (GameSession.Instance.IsResponseRequired)
        {
            // Just send all selected cards for now. Usually, it's 1 card to defend/duel.
            Debug.Log($"[UIController] Playing response cards automatically.");
            SendPlayRequest(selectedIds, new System.Collections.Generic.List<int>());
            return;
        }
    }

    // A helper to send play requests with possibly multiple cards if the protocol supports it, 
    // but right now RequestPlayCard takes 1 cardId. We will send the first one. 
    // To support multiple discards later, you would update the network packet.
    private void SendPlayRequest(System.Collections.Generic.List<int> cardIds, System.Collections.Generic.List<int> targetIds)
    {
        if (cardIds.Count == 0) return;

        RequestPlayCard req = new RequestPlayCard();
        // Sending the first card. (Multi-card discard will need a new packet/request type)
        req.Send(cardIds[0], targetIds);
        NetworkManager.Instance.SendRequest(req);

        // Clear selection
        if (_handManager != null) {
            _handManager.ClearSelection();
        }

        UpdateUIState();
    }
}
