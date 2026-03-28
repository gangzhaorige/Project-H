using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectH.Models;

public class HandManager : MonoBehaviour
{
    // --- Positions (WorldCanvas) ---
    private RectTransform deckPosition;
    private RectTransform discardPilePosition;
    private RectTransform playFieldPanel;

    // --- UI Containers ---
    private RectTransform handPanel;

    [Header("Prefabs")]
    public GameObject cardPrefab;

    // --- Components ---
    private DynamicHandLayout _layoutHandler;

    [Header("Animation Settings")]
    public float drawAnimDuration = 0.5f;

    // --- Hand State ---
    private List<GameObject> orderedCards = new List<GameObject>();
    private List<int> orderedCardIds = new List<int>();
    private Dictionary<int, GameObject> cardMap = new Dictionary<int, GameObject>();
    private List<int> selectedCardIds = new List<int>();

    // --- Dependencies ---
    private CardAnimationManager _cardAnimationManager;
    private AnimationController _animationController;

    public delegate void SelectionChanged();
    public event SelectionChanged OnSelectionChanged;

    public void Init(CardAnimationManager cardAnimationManager, AnimationController animationController, ProjectH.UI.WorldCanvasView canvasView)
    {
        _cardAnimationManager = cardAnimationManager;
        _animationController = animationController;

        if (canvasView != null)
        {
            deckPosition = canvasView.deckPosition;
            discardPilePosition = canvasView.discardPilePosition;
            playFieldPanel = canvasView.playFieldPanel;
            handPanel = canvasView.handPanel;
            _layoutHandler = canvasView.layoutHandHandler;
        }
    }

    // =========================================================================
    //  SELECTION LOGIC
    // =========================================================================

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

    // =========================================================================
    //  HAND MANAGEMENT
    // =========================================================================

    public Vector3 GetPredictiveWorldPosition(int index, int futureTotal)
    {
        if (_layoutHandler == null) return handPanel.position;
        Vector3 local = _layoutHandler.GetLocalPosition(index, futureTotal, handPanel.rect.width);
        return handPanel.TransformPoint(local);
    }

    public void RegisterAnimatedCard(CardData data, GameObject cardGO)
    {
        if (cardMap.ContainsKey(data.Id))
        {
            Destroy(cardGO);
            return;
        }

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
            ui.enabled = true; 
            ui.Bind(data, this);
        }

        cardMap.Add(data.Id, cardGO);
        orderedCards.Add(cardGO);
        orderedCardIds.Add(data.Id);

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

            if (_cardAnimationManager != null)
            {
                _cardAnimationManager.StopAllAnimationsFor(go);
            }

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
        if (_layoutHandler == null) return;

        int countToUse = (futureCount != -1) ? futureCount : orderedCards.Count;
        var targets = _layoutHandler.GetCardTransforms(countToUse, handPanel.rect.width);

        for (int i = 0; i < orderedCards.Count; i++)
        {
            GameObject cardGO = orderedCards[i];
            if (cardGO != null)
            {
                Vector3 targetPos = targets[i].pos;
                
                if (_cardAnimationManager != null)
                {
                    StartCoroutine(_cardAnimationManager.SmoothMoveLocal(cardGO.transform, targetPos, targets[i].rot, 0.3f));
                }
            }
        }
    }

    public void ClearHand()
    {
        foreach (var card in cardMap.Values)
        {
            if (_cardAnimationManager != null) _cardAnimationManager.StopAllAnimationsFor(card);
            Destroy(card);
        }
        cardMap.Clear();
        orderedCards.Clear();
        orderedCardIds.Clear();
        selectedCardIds.Clear();
    }

    // =========================================================================
    //  GLOBAL CARD ANIMATION LOGIC (Formerly CardManager)
    // =========================================================================

    public void HandleLocalDraw(List<CardData> newCards)
    {
        if (_animationController != null) _animationController.AddAnimation(AnimateLocalDrawBatch(newCards));
    }

    public void HandleOtherDraw(int playerId, int cardCount)
    {
        if (GameSession.Instance.Players.TryGetValue(playerId, out PlayerData pData))
        {
            if (_animationController != null) _animationController.AddAnimation(AnimateOtherDrawBatch(pData, cardCount));
        }
    }

    public void HandleMoveCard(ResponseMoveCardEventArgs res)
    {
        if (_animationController != null) _animationController.AddAnimation(AnimateMoveCard(res));
    }

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
                cardGO = GetCardObject(cardData.Id);
                if (cardGO != null)
                {
                    UnregisterCard(cardData.Id);
                    sourcePlayer?.RemoveCardById(cardData.Id);
                    startPos = cardGO.transform.position;
                }
                else
                {
                    startPos = (sourcePlayer?.ChampionObject != null) ? sourcePlayer.ChampionObject.transform.position : Vector3.zero;
                }
            }
            else
            {
                startPos = (sourcePlayer?.ChampionObject != null) ? sourcePlayer.ChampionObject.transform.position : Vector3.zero;
                cardGO = Instantiate(cardPrefab, playFieldPanel); 
                cardGO.transform.position = startPos;
                
                CardSetup setup = cardGO.GetComponent<CardSetup>();
                if (setup != null)
                {
                    if (res.ShowDetails) setup.Init(cardData.Type, cardData.Suit, cardData.Value);
                    else setup.Init(0, 0, 0); 
                }
            }

            var ui = cardGO.GetComponent<CardUIController>();
            if (ui != null) ui.enabled = false;

            // --- 2. Determine End Position ---
            if (localId == res.CasterId && receiverPlayer != null)
            {
                int futureCount = receiverPlayer.Hand.Count + res.Cards.Count;
                int finalIdx = receiverPlayer.Hand.Count + i;
                endPos = GetPredictiveWorldPosition(finalIdx, futureCount);
            }
            else
            {
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
        if (_cardAnimationManager != null)
        {
            yield return _cardAnimationManager.SmoothMoveWorld(cardGO.transform, targetPos, drawAnimDuration);
        }
        else
        {
            yield return new WaitForSeconds(drawAnimDuration);
        }

        if (isReceiver)
        {
            RegisterAnimatedCard(data, cardGO);
            receiver?.AddCard(data);
        }
        else
        {
            if (_cardAnimationManager != null) _cardAnimationManager.StopAllAnimationsFor(cardGO);
            Destroy(cardGO);
            receiver?.AddCard(new CardData { Id = -1 });
        }
    }

    private IEnumerator AnimateLocalDrawBatch(List<CardData> newCards)
    {
        int batchSize = newCards.Count;
        int currentHandCount = GameSession.Instance.GetLocalPlayer().Hand.Count;
        int totalFutureCount = currentHandCount + batchSize;

        ReorganizeHand(totalFutureCount);

        List<Coroutine> batchRoutines = new List<Coroutine>();
        for (int i = 0; i < batchSize; i++)
        {
            int finalIndex = currentHandCount + i;
            batchRoutines.Add(StartCoroutine(AnimateSingleLocalDraw(newCards[i], finalIndex, totalFutureCount)));
        }

        foreach (var routine in batchRoutines)
        {
            yield return routine;
        }
        
        Debug.Log($"[HandManager] Local Draw Batch of {batchSize} completed.");
    }

    private IEnumerator AnimateSingleLocalDraw(CardData card, int finalIndex, int totalFutureCount)
    {
        Debug.Log("------------------------------------");
        Vector3 targetWorldPos = GetPredictiveWorldPosition(finalIndex, totalFutureCount);

        GameObject animCard = Instantiate(cardPrefab, deckPosition != null ? deckPosition.parent : transform);
        Debug.LogError(animCard == null);
        if (deckPosition != null)
        {
            animCard.transform.position = deckPosition.position;
            animCard.transform.rotation = deckPosition.rotation;
        }
        
        CardSetup setup = animCard.GetComponent<CardSetup>();
        if (setup != null) setup.Init(card.Type, card.Suit, card.Value);

        if (AudioManager.Instance != null) AudioManager.Instance.PlayCardDrawSFX();

        if (_cardAnimationManager != null)
        {
            yield return _cardAnimationManager.SmoothMoveWorld(animCard.transform, targetWorldPos, drawAnimDuration);
        }
        else
        {
            yield return new WaitForSeconds(drawAnimDuration);
        }

        RegisterAnimatedCard(card, animCard);
        GameSession.Instance.GetLocalPlayer().AddCard(card);
    }

    private IEnumerator AnimateOtherDrawBatch(PlayerData pData, int count)
    {
        List<Coroutine> batchRoutines = new List<Coroutine>();
        for (int i = 0; i < count; i++)
        {
            batchRoutines.Add(StartCoroutine(AnimateSingleOtherDraw(pData)));
        }

        foreach (var routine in batchRoutines)
        {
            yield return routine;
        }
        Debug.Log($"[HandManager] Other Draw Batch for {pData.Username} completed.");
    }

    private IEnumerator AnimateSingleOtherDraw(PlayerData pData)
    {
        GameObject animCard = Instantiate(cardPrefab, deckPosition != null ? deckPosition.parent : transform);
        if (deckPosition != null) animCard.transform.position = deckPosition.position;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayCardDrawSFX();
        
        Vector3 targetPos = (pData.ChampionObject != null) 
            ? pData.ChampionObject.transform.position 
            : Vector3.zero;

        if (_cardAnimationManager != null)
        {
            yield return _cardAnimationManager.SmoothMoveWorld(animCard.transform, targetPos, drawAnimDuration);
        }
        else
        {
            yield return new WaitForSeconds(drawAnimDuration);
        }
        
        if (_cardAnimationManager != null) _cardAnimationManager.StopAllAnimationsFor(animCard);
        Destroy(animCard);
        pData.AddCard(new CardData { Id = -1 }); 
    }
}
