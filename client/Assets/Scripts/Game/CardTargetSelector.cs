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

    private void Awake()
    {
        Instance = this;
    }

    public void BeginTargeting(CardData card)
    {
        currentCard = card;
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

    public int GetMaxTargets(string type) {
        return CardRuleManager.GetMaxTargets(new CardData { Type = type });
    }

    public void ConfirmTargeting(List<int> targetIds)
    {
        if (currentCard == null) return;

        RequestPlayCard req = new RequestPlayCard();
        req.Send(currentCard.Id, targetIds);
        NetworkManager.Instance.SendRequest(req);
        
        Debug.Log($"[Targeting] Request sent for card {currentCard.Id} with {targetIds.Count} targets.");
        currentCard = null;
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
