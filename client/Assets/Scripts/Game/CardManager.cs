using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectH.Models;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [Header("Positions (WorldCanvas)")]
    public RectTransform deckPosition;
    public RectTransform discardPilePosition;
    public RectTransform playFieldPanel;

    [Header("Components")]
    public HandManager handManager;
    public GameObject cardPrefab;

    [Header("Animation Settings")]
    public float drawAnimDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Enqueues a local draw animation batch to the central controller.
    /// </summary>
    public void HandleLocalDraw(List<CardData> newCards)
    {
        AnimationController.Instance.AddAnimation(AnimateLocalDrawBatch(newCards));
    }

    /// <summary>
    /// Enqueues an animation batch for another player drawing cards.
    /// </summary>
    public void HandleOtherDraw(int playerId, int cardCount)
    {
        if (GameSession.Instance.Players.TryGetValue(playerId, out PlayerData pData))
        {
            AnimationController.Instance.AddAnimation(AnimateOtherDrawBatch(pData, cardCount));
        }
    }

    // --- Private Animation Logic ---

    private IEnumerator AnimateLocalDrawBatch(List<CardData> newCards)
    {
        int batchSize = newCards.Count;
        int currentHandCount = GameSession.Instance.GetLocalPlayer().Hand.Count;
        int totalFutureCount = currentHandCount + batchSize;

        // 1. Instantly move existing cards to their new "spaced out" positions
        handManager.ReorganizeHand(totalFutureCount);

        // 2. Start all draw animations at once
        List<Coroutine> batchRoutines = new List<Coroutine>();
        for (int i = 0; i < batchSize; i++)
        {
            int finalIndex = currentHandCount + i;
            batchRoutines.Add(StartCoroutine(AnimateSingleLocalDraw(newCards[i], finalIndex, totalFutureCount)));
        }

        // 3. Block the AnimationController queue until every card in this batch has landed
        foreach (var routine in batchRoutines)
        {
            yield return routine;
        }
        
        Debug.Log($"[CardManager] Local Draw Batch of {batchSize} completed.");
    }

    private IEnumerator AnimateSingleLocalDraw(CardData card, int finalIndex, int totalFutureCount)
    {
        Vector3 targetWorldPos = handManager.GetPredictiveWorldPosition(finalIndex, totalFutureCount);

        GameObject animCard = Instantiate(cardPrefab, deckPosition.parent);
        animCard.transform.position = deckPosition.position;
        animCard.transform.rotation = deckPosition.rotation;
        
        CardSetup setup = animCard.GetComponent<CardSetup>();
        if (setup != null) setup.Init(card.Type, card.Suit, card.Value);

        yield return MoveCard(animCard.transform, targetWorldPos, drawAnimDuration);

        // Finalize by giving the object to HandManager and updating local data
        handManager.RegisterAnimatedCard(card, animCard);
        GameSession.Instance.GetLocalPlayer().Hand.Add(card);
    }

    private IEnumerator AnimateOtherDrawBatch(PlayerData pData, int count)
    {
        List<Coroutine> batchRoutines = new List<Coroutine>();
        for (int i = 0; i < count; i++)
        {
            batchRoutines.Add(StartCoroutine(AnimateSingleOtherDraw(pData)));
        }

        // Wait for all "other" cards to reach the player
        foreach (var routine in batchRoutines)
        {
            yield return routine;
        }
        Debug.Log($"[CardManager] Other Draw Batch for {pData.Username} completed.");
    }

    private IEnumerator AnimateSingleOtherDraw(PlayerData pData)
    {
        GameObject animCard = Instantiate(cardPrefab, deckPosition.parent);
        animCard.transform.position = deckPosition.position;
        
        Vector3 targetPos = (pData.ChampionObject != null) 
            ? pData.ChampionObject.transform.position 
            : Vector3.zero;

        yield return MoveCard(animCard.transform, targetPos, drawAnimDuration);
        
        Destroy(animCard);
        pData.Hand.Add(new CardData { Id = -1 }); 
    }

    public IEnumerator MoveCard(Transform cardTr, Vector3 targetPos, float duration)
    {
        Vector3 startPos = cardTr.position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            if (cardTr == null) yield break;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t); // SmoothStep interpolation
            cardTr.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        if (cardTr != null) cardTr.position = targetPos;
    }
}
