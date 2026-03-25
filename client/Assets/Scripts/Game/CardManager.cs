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

    public void HandleMoveCard(ResponseMoveCardEventArgs res)
    {
        AnimationController.Instance.AddAnimation(AnimateMoveCard(res));
    }

    // --- Private Animation Logic ---

    private IEnumerator AnimateMoveCard(ResponseMoveCardEventArgs res)
    {
        int localId = Constants.USER_ID;
        PlayerData sourcePlayer = GameSession.Instance.Players.ContainsKey(res.TargetId) ? GameSession.Instance.Players[res.TargetId] : null;
        PlayerData receiverPlayer = GameSession.Instance.Players.ContainsKey(res.CasterId) ? GameSession.Instance.Players[res.CasterId] : null;

        List<Coroutine> anims = new List<Coroutine>();

        for (int i = 0; i < res.Cards.Count; i++)
        {
            CardData cardData = res.Cards[i];
            GameObject cardGO = null;
            Vector3 startPos;
            Vector3 endPos;

            // --- 1. Identify/Create Card Object and Start Position ---
            if (localId == res.TargetId)
            {
                // Target: find card in hand
                cardGO = handManager.GetCardObject(cardData.Id);
                if (cardGO != null)
                {
                    handManager.UnregisterCard(cardData.Id);
                    sourcePlayer.RemoveCardById(cardData.Id);
                    startPos = cardGO.transform.position;
                }
                else
                {
                    // Fallback if not found
                    startPos = (sourcePlayer?.ChampionObject != null) ? sourcePlayer.ChampionObject.transform.position : Vector3.zero;
                }
            }
            else
            {
                // Caster or Observer: create placeholder at source player champion
                startPos = (sourcePlayer?.ChampionObject != null) ? sourcePlayer.ChampionObject.transform.position : Vector3.zero;
                cardGO = Instantiate(cardPrefab, playFieldPanel); // Use playfield as temp parent
                cardGO.transform.position = startPos;
                
                CardSetup setup = cardGO.GetComponent<CardSetup>();
                if (setup != null)
                {
                    if (res.ShowDetails) setup.Init(cardData.Type, cardData.Suit, cardData.Value);
                    else setup.Init(0, 0, 0); // Hidden
                }
            }

            // Disable interactions during flight
            var ui = cardGO.GetComponent<CardUIController>();
            if (ui != null) ui.enabled = false;

            // --- 2. Determine End Position ---
            if (localId == res.CasterId)
            {
                // Caster: animate to hand
                int futureCount = receiverPlayer.Hand.Count + res.Cards.Count;
                int finalIdx = receiverPlayer.Hand.Count + i;
                endPos = handManager.GetPredictiveWorldPosition(finalIdx, futureCount);
            }
            else
            {
                // Target or Observer: animate to receiver champion
                endPos = (receiverPlayer?.ChampionObject != null) ? receiverPlayer.ChampionObject.transform.position : Vector3.down * 1000;
            }

            // --- 3. Execute Animation ---
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCardMoveSFX();
            anims.Add(StartCoroutine(MoveAndFinalize(cardGO, cardData, endPos, localId == res.CasterId, receiverPlayer)));
        }

        foreach (var a in anims) yield return a;
    }

    private IEnumerator MoveAndFinalize(GameObject cardGO, CardData data, Vector3 targetPos, bool isReceiver, PlayerData receiver)
    {
        if (CardAnimationManager.Instance != null)
        {
            yield return CardAnimationManager.Instance.SmoothMoveWorld(cardGO.transform, targetPos, drawAnimDuration);
        }
        else
        {
            yield return new WaitForSeconds(drawAnimDuration);
        }

        if (isReceiver)
        {
            // Receiver: register to hand
            handManager.RegisterAnimatedCard(data, cardGO);
            receiver.AddCard(data);
        }
        else
        {
            // Target/Observer: destroy
            if (CardAnimationManager.Instance != null) CardAnimationManager.Instance.StopAllAnimationsFor(cardGO);
            Destroy(cardGO);
            if (receiver != null) receiver.AddCard(new CardData { Id = -1 });
        }
    }

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

        if (AudioManager.Instance != null) AudioManager.Instance.PlayCardDrawSFX();

        if (CardAnimationManager.Instance != null)
        {
            yield return CardAnimationManager.Instance.SmoothMoveWorld(animCard.transform, targetWorldPos, drawAnimDuration);
        }
        else
        {
            yield return new WaitForSeconds(drawAnimDuration);
        }

        // Finalize by giving the object to HandManager and updating local data
        handManager.RegisterAnimatedCard(card, animCard);
        GameSession.Instance.GetLocalPlayer().AddCard(card);
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

        if (AudioManager.Instance != null) AudioManager.Instance.PlayCardDrawSFX();
        
        Vector3 targetPos = (pData.ChampionObject != null) 
            ? pData.ChampionObject.transform.position 
            : Vector3.zero;

        if (CardAnimationManager.Instance != null)
        {
            yield return CardAnimationManager.Instance.SmoothMoveWorld(animCard.transform, targetPos, drawAnimDuration);
        }
        else
        {
            yield return new WaitForSeconds(drawAnimDuration);
        }
        
        if (CardAnimationManager.Instance != null) CardAnimationManager.Instance.StopAllAnimationsFor(animCard);
        Destroy(animCard);
        pData.AddCard(new CardData { Id = -1 }); 
    }
}
