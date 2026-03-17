using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class HandManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("UI Containers")]
    public RectTransform handPanel;

    [Header("Components")]
    public DynamicHandLayout layoutHandler;

    [Header("Hover Settings")]
    public float hoverYOffset = 40f;

    private List<GameObject> orderedCards = new List<GameObject>();
    private List<int> orderedCardIds = new List<int>();
    private Dictionary<int, GameObject> cardMap = new Dictionary<int, GameObject>();
    private Dictionary<int, Coroutine> activeCoroutines = new Dictionary<int, Coroutine>();
    private int hoveredCardId = -1;

    public void SetHoveredCard(int cardId)
    {
        if (hoveredCardId == cardId) return;
        hoveredCardId = cardId;
        
        // Just bring to front, don't re-run full layout
        if (cardMap.TryGetValue(cardId, out GameObject cardGO))
        {
            cardGO.transform.SetAsLastSibling();
        }
    }

    public void ClearHoveredCard(int cardId)
    {
        if (hoveredCardId == cardId)
        {
            hoveredCardId = -1;
            // No need to reorganize, CardUIController will reset its own container
        }
    }

    /// <summary>
    /// Predicts where a card will end up in world space given its index and future total count.
    /// </summary>
    public Vector3 GetPredictiveWorldPosition(int index, int futureTotal)
    {
        if (layoutHandler == null) return handPanel.position;
        Vector3 local = layoutHandler.GetLocalPosition(index, futureTotal, handPanel.rect.width);
        return handPanel.TransformPoint(local);
    }

    /// <summary>
    /// Registers a card that has already been instantiated and animated.
    /// </summary>
    public void RegisterAnimatedCard(CardData data, GameObject cardGO)
    {
        if (cardMap.ContainsKey(data.Id))
        {
            Destroy(cardGO);
            return;
        }

        // Set parent and restore world position manually to handle potential parent scale differences
        Vector3 worldPos = cardGO.transform.position;
        Quaternion worldRot = cardGO.transform.rotation;
        cardGO.transform.SetParent(handPanel, false);
        cardGO.transform.position = worldPos;
        cardGO.transform.rotation = worldRot;

        cardGO.name = $"Card_{data.Type}_{data.Id}";
        
        CardSetup setup = cardGO.GetComponent<CardSetup>();
        if (setup != null) setup.Init(data.Type, data.Suit, data.Value);

        CardUIController ui = cardGO.GetComponent<CardUIController>();
        if (ui != null) 
        {
            ui.Bind(data);
        }
        else
        {
            Debug.LogWarning($"[HandManager] CardUIController component missing on prefab for card {data.Id}!");
        }

        cardMap.Add(data.Id, cardGO);
        orderedCards.Add(cardGO);
        orderedCardIds.Add(data.Id);

        // Crucial: immediately update the layout once the card is officially in the hand
        ReorganizeHand();
    }

    public void RemoveCard(int cardId)
    {
        if (cardMap.TryGetValue(cardId, out GameObject go))
        {
            int index = orderedCards.IndexOf(go);
            if (index != -1)
            {
                orderedCards.RemoveAt(index);
                orderedCardIds.RemoveAt(index);
            }

            cardMap.Remove(cardId);

            if (activeCoroutines.TryGetValue(cardId, out Coroutine co))
            {
                StopCoroutine(co);
                activeCoroutines.Remove(cardId);
            }

            if (hoveredCardId == cardId) hoveredCardId = -1;

            Destroy(go);
            ReorganizeHand();
        }
    }

    public void ReorganizeHand(int futureCount = -1)
    {
        if (layoutHandler == null) return;

        int countToUse = (futureCount != -1) ? futureCount : orderedCards.Count;
        var targets = layoutHandler.GetCardTransforms(countToUse, handPanel.rect.width);

        for (int i = 0; i < orderedCards.Count; i++)
        {
            GameObject cardGO = orderedCards[i];
            if (cardGO != null)
            {
                int cardId = orderedCardIds[i];
                Vector3 targetPos = targets[i].pos;

                // Move hovered card to front so it's not hidden by neighbors
                if (cardId == hoveredCardId)
                {
                    cardGO.transform.SetAsLastSibling();
                }

                // Stop previous movement to avoid "Animation Conflict" (stacking)
                if (activeCoroutines.TryGetValue(cardId, out Coroutine co))
                {
                    StopCoroutine(co);
                }

                activeCoroutines[cardId] = StartCoroutine(SmoothMove(cardGO.transform, targetPos, targets[i].rot, 0.3f, cardId));
            }
        }
    }

    private System.Collections.IEnumerator SmoothMove(Transform tr, Vector3 targetLocalPos, Quaternion targetLocalRot, float duration, int cardId)
    {
        Vector3 startPos = tr.localPosition;
        Quaternion startRot = tr.localRotation;
        float elapsed = 0;

        while (elapsed < duration)
        {
            if (tr == null) yield break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smooth step

            tr.localPosition = Vector3.Lerp(startPos, targetLocalPos, t);
            tr.localRotation = Quaternion.Slerp(startRot, targetLocalRot, t);
            yield return null;
        }

        if (tr != null)
        {
            tr.localPosition = targetLocalPos;
            tr.localRotation = targetLocalRot;
        }

        activeCoroutines.Remove(cardId);
    }

    public void ClearHand()
    {
        StopAllCoroutines();
        foreach (var card in cardMap.Values) Destroy(card);
        cardMap.Clear();
        orderedCards.Clear();
        orderedCardIds.Clear();
        activeCoroutines.Clear();
        hoveredCardId = -1;
    }
}
