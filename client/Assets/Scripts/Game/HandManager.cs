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

    private List<GameObject> orderedCards = new List<GameObject>();
    private Dictionary<int, GameObject> cardMap = new Dictionary<int, GameObject>();

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

        cardGO.transform.SetParent(handPanel, true);
        cardGO.name = $"Card_{data.Type}_{data.Id}";
        
        CardSetup setup = cardGO.GetComponent<CardSetup>();
        if (setup != null) setup.Init(data.Type, data.Suit, data.Value);

        cardMap.Add(data.Id, cardGO);
        orderedCards.Add(cardGO);
        
        // Re-organize to ensure all other cards are in their correct slots
        ReorganizeHand();
    }

    public void RemoveCard(int cardId)
    {
        if (cardMap.TryGetValue(cardId, out GameObject go))
        {
            orderedCards.Remove(go);
            cardMap.Remove(cardId);
            Destroy(go);
            ReorganizeHand();
        }
    }

    public void ReorganizeHand()
    {
        if (layoutHandler == null) return;

        var targets = layoutHandler.GetCardTransforms(orderedCards.Count, handPanel.rect.width);

        for (int i = 0; i < orderedCards.Count; i++)
        {
            if (orderedCards[i] != null)
            {
                // Note: Using a shorter duration for re-aligning existing cards
                StartCoroutine(SmoothMove(orderedCards[i].transform, targets[i].pos, targets[i].rot, 0.2f));
            }
        }
    }

    private System.Collections.IEnumerator SmoothMove(Transform tr, Vector3 targetLocalPos, Quaternion targetLocalRot, float duration)
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
    }

    public void ClearHand()
    {
        StopAllCoroutines();
        foreach (var card in cardMap.Values) Destroy(card);
        cardMap.Clear();
        orderedCards.Clear();
    }
}
