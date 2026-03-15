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

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Animates cards being drawn by the local player.
    /// </summary>
    public void HandleLocalDraw(List<CardData> newCards)
    {
        for (int i = 0; i < newCards.Count; i++)
        {
            StartCoroutine(AnimateSingleLocalDraw(newCards[i], i, newCards.Count));
        }
    }

    /// <summary>
    /// Animates cards being drawn by another player.
    /// </summary>
    public void HandleOtherDraw(int playerId, int cardCount)
    {
        if (GameSession.Instance.Players.TryGetValue(playerId, out PlayerData pData))
        {
            for (int i = 0; i < cardCount; i++)
            {
                StartCoroutine(AnimateSingleOtherDraw(pData, i, cardCount));
            }
        }
    }

    private IEnumerator AnimateSingleLocalDraw(CardData card, int index, int total)
    {
        // Create a temporary card for animation
        GameObject animCard = Instantiate(cardPrefab, deckPosition.parent);
        
        // Calculate a slight offset based on index so they spread out during flight
        float totalWidth = (total - 1) * cardSpacingOffset;
        Vector3 offset = new Vector3((index * cardSpacingOffset) - (totalWidth / 2f), 0, 0);
        
        animCard.transform.position = deckPosition.position + offset;
        animCard.transform.rotation = deckPosition.rotation;
        
        // Initialize visuals
        CardSetup setup = animCard.GetComponent<CardSetup>();
        if (setup != null) setup.Init(card.Type, card.Suit, card.Value);

        // Move to hand
        yield return MoveCard(animCard.transform, handManager.handPanel.position, drawAnimDuration);

        // Add to actual hand manager and destroy temp
        handManager.AddCard(card);
        GameSession.Instance.GetLocalPlayer().Hand.Add(card);
        Destroy(animCard);
    }

    private IEnumerator AnimateSingleOtherDraw(PlayerData pData, int index, int total)
    {
        // Create a placeholder card
        GameObject animCard = Instantiate(cardPrefab, deckPosition.parent);
        
        float totalWidth = (total - 1) * cardSpacingOffset;
        Vector3 offset = new Vector3((index * cardSpacingOffset) - (totalWidth / 2f), 0, 0);
        
        animCard.transform.position = deckPosition.position + offset;
        animCard.transform.rotation = deckPosition.rotation;
        
        Vector3 targetPos = Vector3.zero;
        if (pData.ChampionObject != null)
        {
            targetPos = pData.ChampionObject.transform.position;
        }

        // Move and scale down to zero to simulate disappearing into hand
        yield return MoveAndScaleCard(animCard.transform, targetPos, Vector3.zero, drawAnimDuration);

        // Update session data
        pData.Hand.Add(new CardData { Id = -1 }); 
        
        Destroy(animCard);
    }

    private IEnumerator MoveAndScaleCard(Transform cardTr, Vector3 targetPos, Vector3 targetScale, float duration)
    {
        Vector3 startPos = cardTr.position;
        Vector3 startScale = cardTr.localScale;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smooth step
            
            cardTr.position = Vector3.Lerp(startPos, targetPos, t);
            cardTr.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        cardTr.position = targetPos;
        cardTr.localScale = targetScale;
    }

    private IEnumerator MoveCard(Transform cardTr, Vector3 targetPos, float duration)
    {
        Vector3 startPos = cardTr.position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            cardTr.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        cardTr.position = targetPos;
    }
}
