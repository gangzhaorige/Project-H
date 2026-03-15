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
    public float cardSpacingOffset = 0.2f;

    private int cardsCurrentlyDrawing = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void HandleLocalDraw(List<CardData> newCards)
    {
        int batchSize = newCards.Count;
        int currentHandCount = GameSession.Instance.GetLocalPlayer().Hand.Count;
        
        for (int i = 0; i < batchSize; i++)
        {
            // Predict the final index this card will have in the hand
            int finalIndex = currentHandCount + i;
            int totalFutureCount = currentHandCount + batchSize;
            
            StartCoroutine(AnimateSingleLocalDraw(newCards[i], finalIndex, totalFutureCount));
        }
    }

    private IEnumerator AnimateSingleLocalDraw(CardData card, int finalIndex, int totalFutureCount)
    {
        // 1. Get the EXACT final world position from HandManager
        Vector3 targetWorldPos = handManager.GetPredictiveWorldPosition(finalIndex, totalFutureCount);

        // 2. Create the card at deck
        GameObject animCard = Instantiate(cardPrefab, deckPosition.parent);
        animCard.transform.position = deckPosition.position;
        animCard.transform.rotation = deckPosition.rotation;
        
        // Initialize visuals immediately so it's not a white box during flight
        CardSetup setup = animCard.GetComponent<CardSetup>();
        if (setup != null) setup.Init(card.Type, card.Suit, card.Value);

        // 3. Move directly to the final hand position
        // We also trigger a reorganization of existing cards so they move out of the way
        handManager.ReorganizeHand(); 
        
        yield return MoveCard(animCard.transform, targetWorldPos, drawAnimDuration);

        // 4. Finalize
        handManager.RegisterAnimatedCard(card, animCard);
        GameSession.Instance.GetLocalPlayer().Hand.Add(card);
    }

    public void HandleOtherDraw(int playerId, int cardCount)
    {
        if (GameSession.Instance.Players.TryGetValue(playerId, out PlayerData pData))
        {
            for (int i = 0; i < cardCount; i++)
            {
                StartCoroutine(AnimateSingleOtherDraw(pData));
            }
        }
    }

    private IEnumerator AnimateSingleOtherDraw(PlayerData pData)
    {
        GameObject animCard = Instantiate(cardPrefab, deckPosition.parent);
        animCard.transform.position = deckPosition.position;
        
        Vector3 targetPos = Vector3.zero;
        if (pData.ChampionObject != null)
        {
            targetPos = pData.ChampionObject.transform.position;
        }

        yield return MoveCard(animCard.transform, targetPos, drawAnimDuration);
        Destroy(animCard);

        pData.Hand.Add(new CardData { Id = -1 }); 
    }

    private IEnumerator MoveCard(Transform cardTr, Vector3 targetPos, float duration)
    {
        Vector3 startPos = cardTr.position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            if (cardTr == null) yield break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            cardTr.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        if (cardTr != null) cardTr.position = targetPos;
    }
}
