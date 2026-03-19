using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using ProjectH.Models;

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
        int maxTargets = GetMaxTargets(card.Type);
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
        switch (type) {
            case "ATTACK": case "DUEL": return 1;
            case "ARROW": case "FIRE": case "HEAL_ALL": return 0; // AOE cards skip target selection
            case "DODGE": case "HEAL": case "NEGATE": case "DRAW": return 0; // Self/No-target cards skip target selection
            default: return 0;
        }
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

        int distance = GetDistance(attackerId, targetId);
        int effectiveDistance = distance + target.Champion.SpecialDefense;

        Debug.Log($"[RangeCheck] Attacker {attackerId} (Range: {attacker.Champion.AttackRange}) -> Target {targetId} (Defense: {target.Champion.SpecialDefense}). Distance: {distance}, Effective: {effectiveDistance}");

        return effectiveDistance <= attacker.Champion.AttackRange;
    }

    private int GetDistance(int p1Id, int p2Id)
    {
        List<int> order = GameSession.Instance.PlayerOrder;
        int idx1 = order.IndexOf(p1Id);
        int idx2 = order.IndexOf(p2Id);

        if (idx1 == -1 || idx2 == -1) return 999;

        int n = order.Count;
        int diff = Mathf.Abs(idx1 - idx2);
        
        // Shortest distance in a circle
        return Mathf.Min(diff, n - diff);
    }
}
