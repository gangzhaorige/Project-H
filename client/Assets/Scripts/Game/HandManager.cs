using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("UI Containers")]
    public RectTransform handPanel;

    [Header("Components")]
    public DynamicHandLayout layoutHandler;

    private List<GameObject> orderedCards = new List<GameObject>();
    private List<int> orderedCardIds = new List<int>();
    private Dictionary<int, GameObject> cardMap = new Dictionary<int, GameObject>();
    private Dictionary<int, Coroutine> activeCoroutines = new Dictionary<int, Coroutine>();
    private List<int> selectedCardIds = new List<int>();

    public delegate void SelectionChanged();
    public event SelectionChanged OnSelectionChanged;

    private void Awake()
    {
        Instance = this;
    }

    public void ToggleCardSelection(int cardId)
    {
        if (selectedCardIds.Contains(cardId))
        {
            selectedCardIds.Remove(cardId);
        }
        else
        {
            selectedCardIds.Add(cardId);
        }

        OnSelectionChanged?.Invoke();
    }

    public bool IsSelected(int cardId)
    {
        return selectedCardIds.Contains(cardId);
    }

    public List<int> GetSelectedCardIds()
    {
        return new List<int>(selectedCardIds);
    }

    public void ClearSelection()
    {
        selectedCardIds.Clear();
        OnSelectionChanged?.Invoke();
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
            ui.enabled = true; // Enable interaction when in hand
            ui.Bind(data, this);
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

    public GameObject GetCardObject(int cardId)
    {
        if (cardMap.TryGetValue(cardId, out GameObject go)) return go;
        return null;
    }

    public int GetCardIndex(int cardId)
    {
        if (cardMap.TryGetValue(cardId, out GameObject go))
        {
            return orderedCards.IndexOf(go);
        }
        return -1;
    }

    /// <summary>
    /// Registers a card that has already been instantiated and animated at a specific index.
    /// </summary>
    public void RegisterAnimatedCardAtIndex(CardData data, GameObject cardGO, int index)
    {
        if (cardMap.ContainsKey(data.Id))
        {
            Destroy(cardGO);
            return;
        }

        // Set parent and restore world position manually
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
            ui.enabled = true; // Enable interaction when in hand
            ui.Bind(data, this);
        }

        cardMap.Add(data.Id, cardGO);
        
        // Insert at specified index
        int insertIdx = Mathf.Clamp(index, 0, orderedCards.Count);
        orderedCards.Insert(insertIdx, cardGO);
        orderedCardIds.Insert(insertIdx, data.Id);

        ReorganizeHand();
    }

    /// <summary>
    /// Unregisters a card from the hand logic without destroying it.
    /// Used when a card is played and moves to the play field.
    /// </summary>
    public void UnregisterCard(int cardId)
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

            // Reorganize the remaining cards in the hand
            ReorganizeHand();
        }
    }

    public void RemoveCard(int cardId)
    {
        if (cardMap.TryGetValue(cardId, out GameObject go))
        {
            UnregisterCard(cardId);
            Destroy(go);
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
        selectedCardIds.Clear();
    }
}
