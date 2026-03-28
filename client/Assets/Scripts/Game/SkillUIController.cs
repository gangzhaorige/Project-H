using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectH.Models;
using System.Collections.Generic;

public class SkillUIController : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public Button skillButton; 

    private SkillSO skillData;
    private bool isQueryActive = false;
    private bool isUsedThisTurn = false;

    private HandManager _handManager;
    private CardTargetSelector _cardTargetSelector;

    public void Init(SkillSO skillSO, HandManager handManager, CardTargetSelector cardTargetSelector)
    {
        skillData = skillSO;
        _handManager = handManager;
        _cardTargetSelector = cardTargetSelector;

        if (nameText != null)
        {
            nameText.text = skillData.skillName;
        }

        if (iconImage != null && skillData.skillIcon != null)
        {
            iconImage.sprite = skillData.skillIcon;
        }

        if (skillButton != null)
        {
            skillButton.interactable = false;
            skillButton.onClick.AddListener(OnSkillButtonClick);
        }

        // Subscriptions
        GameSession.Instance.OnSkillQuery += OnSkillQueryReceived;
        GameSession.Instance.OnSkillQueryAnswered += OnSkillQueryAnswered;
        GameSession.Instance.OnTimerCancelled += OnTimerCancelled;
        GameSession.Instance.OnStateChanged += (state) => UpdateInteractableState();
        GameSession.Instance.OnTurnStarted += (id) => { isUsedThisTurn = false; UpdateInteractableState(); };
        
        if (_handManager != null)
        {
            _handManager.OnSelectionChanged += UpdateInteractableState;
        }
        
        UpdateInteractableState();
        Debug.Log($"Initialized Skill UI for: {skillData.skillName}");
    }

    private void OnDestroy()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.OnSkillQuery -= OnSkillQueryReceived;
            GameSession.Instance.OnSkillQueryAnswered -= OnSkillQueryAnswered;
            GameSession.Instance.OnTimerCancelled -= OnTimerCancelled;
        }
        if (_handManager != null)
        {
            _handManager.OnSelectionChanged -= UpdateInteractableState;
        }
    }

    private void UpdateInteractableState()
    {
        if (skillButton == null || skillData == null) return;

        // If the server is querying this specific skill, it takes precedence
        if (isQueryActive)
        {
            skillButton.interactable = true;
            return;
        }

        // Handle Manual Active Skills
        if (!skillData.isPassive)
        {
            skillButton.interactable = CheckManualActivationConditions();
        }
        else
        {
            skillButton.interactable = false;
        }
    }

    private bool CheckManualActivationConditions()
    {
        if (isUsedThisTurn) return false;

        foreach (var condition in skillData.playConditions)
        {
            switch (condition)
            {
                case PlayCondition.IsPlayerTurn:
                    if (GameSession.Instance.ActivePlayerId != Constants.USER_ID) return false;
                    break;
                case PlayCondition.IsPlayerActionState:
                    if (GameSession.Instance.State != "PlayActionState") return false;
                    break;
                case PlayCondition.HandAtleastTwoCards:
                    // This check is usually for total hand size, but based on requirement
                    // "discard two card", we check if exactly 2 are selected
                    if (_handManager != null && _handManager.GetSelectedCardIds().Count < 2) return false;
                    break;
            }
        }

        // Check Card Requirement specifically
        if (skillData.cardRequirement != null && skillData.cardRequirement.discardCount > 0)
        {
            if (_handManager != null && _handManager.GetSelectedCardIds().Count != skillData.cardRequirement.discardCount)
                return false;
        }

        return true;
    }

    private void OnSkillButtonClick()
    {
        if (isQueryActive)
        {
            // Respond to server query
            RequestSkillResponse req = new RequestSkillResponse();
            req.Send(skillData.skillId, true);
            NetworkManager.Instance.SendRequest(req);
            OnSkillQueryAnswered();
        }
        else
        {
            // Manual Activation
            List<int> selectedCards = _handManager != null ? _handManager.GetSelectedCardIds() : new List<int>();
            if (skillData.numberOfTargetToSelect > 0)
            {
                if (_cardTargetSelector != null)
                {
                    _cardTargetSelector.BeginTargetingForSkill(skillData.skillId, selectedCards);
                }
            }
            else
            {
                // Instant activation
                RequestActivateSkill req = new RequestActivateSkill();
                req.Send(skillData.skillId, selectedCards, new List<int>());
                NetworkManager.Instance.SendRequest(req);
                isUsedThisTurn = true;
                UpdateInteractableState();
            }
        }
    }

    private void OnSkillQueryReceived(int querySkillId)
    {
        if (skillData != null && querySkillId == skillData.skillId)
        {
            isQueryActive = true;
            UpdateInteractableState();
        }
    }

    private void OnSkillQueryAnswered()
    {
        isQueryActive = false;
        UpdateInteractableState();
    }

    private void OnTimerCancelled()
    {
        isQueryActive = false;
        UpdateInteractableState();
    }
}
