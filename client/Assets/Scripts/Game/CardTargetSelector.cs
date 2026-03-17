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

    [Header("Settings")]
    public LayerMask targetLayer; // Ensure your Champion prefabs have a collider and are on this layer

    private CardData currentCard;
    private List<int> selectedTargetIds = new List<int>();
    private bool isTargeting = false;

    private void Awake()
    {
        Instance = this;
    }

    public void BeginTargeting(CardData card)
    {
        currentCard = card;
        selectedTargetIds.Clear();
        isTargeting = true;
        
        Debug.Log($"[Targeting] Select targets for {card.Type}. (Click a champion on the field)");
        
        // Simple case: if it's a "Heal" or "Self" card, just send immediately for now
        if (card.Type == "Heal")
        {
            SendPlayRequest();
        }
    }

    private void Update()
    {
        if (!isTargeting) return;

        // Cancel targeting via right click
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            isTargeting = false;
            Debug.Log("[Targeting] Cancelled.");
            return;
        }

        // Selection via Raycast on left click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, targetLayer))
            {
                ChampionController controller = hit.collider.GetComponentInParent<ChampionController>();
                if (controller != null)
                {
                    // Find which player this champion belongs to
                    foreach (var pair in GameSession.Instance.Players)
                    {
                        if (pair.Value.ChampionObject == controller.gameObject)
                        {
                            OnTargetSelected(pair.Key);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void OnTargetSelected(int playerId)
    {
        if (selectedTargetIds.Contains(playerId)) return;

        selectedTargetIds.Add(playerId);
        Debug.Log($"[Targeting] Added target: {playerId}. Count: {selectedTargetIds.Count}");

        // For now, assume most cards take 1 target
        // We will add more complex logic for "maxTarget" later
        SendPlayRequest();
    }

    private void SendPlayRequest()
    {
        isTargeting = false;
        
        RequestPlayCard req = new RequestPlayCard();
        req.Send(currentCard.Id, selectedTargetIds);
        NetworkManager.Instance.SendRequest(req);
        
        Debug.Log($"[Targeting] Request sent for card {currentCard.Id}");
    }
}
