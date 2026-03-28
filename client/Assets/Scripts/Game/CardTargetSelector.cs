using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using ProjectH.Models;
using ProjectH.Rules;

/**
 * CardTargetSelector manages the "Targeting Mode" when a card is clicked.
 */
public class CardTargetSelector : MonoBehaviour
{
    private UIController _uiController;
    private TargetSelectionManager _targetSelectionManager;
    private HandManager _handManager;

    private CardData currentCard;
    private int currentSkillId = -1;
    private List<int> discardCardIds = new List<int>();

    public void Init(UIController uiController, TargetSelectionManager targetSelectionManager, HandManager handManager)
    {
        _uiController = uiController;
        _targetSelectionManager = targetSelectionManager;
        _handManager = handManager;
    }

    public void BeginTargeting(CardData card)
    {
        currentCard = card;
        currentSkillId = -1;
        int maxTargets = CardRuleManager.GetMaxTargets(card);
        Debug.Log($"[Targeting] Showing UI for {card.Type}. MaxTargets: {maxTargets}");

        if (maxTargets == 0)
        {
            ConfirmTargeting(new List<int>());
            return;
        }
        
        if (_uiController != null)
        {
            _uiController.ShowTargetSelectionPanel(true);
            if (_targetSelectionManager != null) _targetSelectionManager.Show(card);
        }
        else
        {
            Debug.LogError("[CardTargetSelector] UIController is missing!");
        }
    }

    public void BeginTargetingForSkill(int skillId, List<int> discards)
    {
        currentSkillId = skillId;
        currentCard = null;
        this.discardCardIds = discards;

        // Bronya's skill ID 10 targets 1 champion
        int maxTargets = (skillId == 10) ? 1 : 0;

        if (_uiController != null)
        {
            _uiController.ShowTargetSelectionPanel(true);
            if (_targetSelectionManager != null) _targetSelectionManager.ShowForSkill(skillId, maxTargets);
        }
    }

    public int GetMaxTargets(int type) {
        return CardRuleManager.GetMaxTargets(new CardData { Type = type });
    }

    public void ConfirmTargeting(List<int> targetIds)
    {
        if (currentSkillId != -1)
        {
            ConfirmTargetingForSkill(targetIds);
            return;
        }

        if (currentCard == null) return;

        RequestPlayCard req = new RequestPlayCard();
        req.Send(currentCard.Id, targetIds);
        NetworkManager.Instance.SendRequest(req);
        
        Debug.Log($"[Targeting] Request sent for card {currentCard.Id} with {targetIds.Count} targets.");
        currentCard = null;

        // Clear selection in HandManager after playing
        if (_handManager != null)
        {
            _handManager.ClearSelection();
        }
    }

    private void ConfirmTargetingForSkill(List<int> targetIds)
    {
        RequestActivateSkill req = new RequestActivateSkill();
        req.Send(currentSkillId, discardCardIds, targetIds);
        NetworkManager.Instance.SendRequest(req);

        Debug.Log($"[Targeting] Request sent for skill {currentSkillId} with {targetIds.Count} targets.");
        currentSkillId = -1;
        discardCardIds.Clear();

        if (_handManager != null)
        {
            _handManager.ClearSelection();
        }
    }

    public void CancelTargeting()
    {
        Debug.Log("[Targeting] Cancelled.");
        currentCard = null;
    }

    public bool CanTarget(int attackerId, int targetId)
    {
        if (attackerId == targetId) return false; // Cannot attack self

        if (!GameSession.Instance.Players.TryGetValue(attackerId, out PlayerData attacker) ||
            !GameSession.Instance.Players.TryGetValue(targetId, out PlayerData target))
        {
            return false;
        }

        if (currentCard == null) return true; // Default to true if no card context

        return CardRuleManager.CanTarget(currentCard, attacker, target);
    }
}
