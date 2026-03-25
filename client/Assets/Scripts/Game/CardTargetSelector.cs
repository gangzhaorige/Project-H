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
    public static CardTargetSelector Instance { get; private set; }

    private CardData currentCard;
    private int currentSkillId = -1;
    private List<int> discardCardIds = new List<int>();

    private void Awake()
    {
        Instance = this;
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
        
        if (UIController.Instance != null)
        {
            UIController.Instance.ShowTargetSelectionPanel(true);
            TargetSelectionUI.Instance.Show(card);
        }
        else
        {
            Debug.LogError("[CardTargetSelector] UIController.Instance is missing!");
        }
    }

    public void BeginTargetingForSkill(int skillId, List<int> discards)
    {
        currentSkillId = skillId;
        currentCard = null;
        this.discardCardIds = discards;

        // Bronya's skill ID 10 targets 1 champion
        int maxTargets = (skillId == 10) ? 1 : 0;

        if (UIController.Instance != null)
        {
            UIController.Instance.ShowTargetSelectionPanel(true);
            TargetSelectionUI.Instance.ShowForSkill(skillId, maxTargets);
        }
    }

    public int GetMaxTargets(string type) {
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
        if (HandManager.Instance != null)
        {
            HandManager.Instance.ClearSelection();
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

        if (HandManager.Instance != null)
        {
            HandManager.Instance.ClearSelection();
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
