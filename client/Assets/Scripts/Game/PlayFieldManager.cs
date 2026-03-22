using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectH.Models;

public class PlayFieldManager : MonoBehaviour
{
    public static PlayFieldManager Instance { get; private set; }

    [Header("UI Containers")]
    public RectTransform playFieldPanel; // Where the card will land
    public HandManager handManager;

    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("Settings")]
    public float playAnimDuration = 0.5f;
    public float cardSpacing = 250f;

    private List<GameObject> cardsOnField = new List<GameObject>();
    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Clears all cards currently on the play field.
    /// To be called by a future response.
    /// </summary>
    public void ClearField()
    {
        foreach (var co in activeCoroutines.Values) StopCoroutine(co);
        activeCoroutines.Clear();

        foreach (GameObject card in cardsOnField)
        {
            if (card != null) Destroy(card);
        }
        cardsOnField.Clear();
    }

    public void ReorganizeField()
    {
        int count = cardsOnField.Count;
        if (count == 0) return;

        // Calculate centering logic
        float totalWidth = (count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            GameObject go = cardsOnField[i];
            if (go == null) continue;

            Vector3 targetLocalPos = new Vector3(startX + (i * cardSpacing), 0, 0);
            
            // Stop existing move if any
            if (activeCoroutines.TryGetValue(go, out Coroutine co))
            {
                StopCoroutine(co);
            }

            activeCoroutines[go] = StartCoroutine(SmoothMove(go.transform, targetLocalPos, Quaternion.identity, playAnimDuration, go));
        }
    }

    private void Start()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.AddCallback(Constants.SMSG_PLAY_CARD, OnResponsePlayCard);
            NetworkManager.Instance.AddCallback(Constants.SMSG_JUDGE, OnJudgementResult);
            NetworkManager.Instance.AddCallback(Constants.SMSG_SWAP_FIELD_HAND, OnResponseSwapFieldHand);
            NetworkManager.Instance.AddCallback(Constants.SMSG_DISCARD_CARDS, OnResponseDiscardCard);
            NetworkManager.Instance.AddCallback(Constants.SMSG_FIELD_TO_HAND, OnResponseFieldToHand);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_PLAY_CARD);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_JUDGE);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_SWAP_FIELD_HAND);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_DISCARD_CARDS);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_FIELD_TO_HAND);
        }
    }

    private void OnResponseFieldToHand(ExtendedEventArgs args)
    {
        ResponseFieldToHandEventArgs res = args as ResponseFieldToHandEventArgs;
        if (res == null) return;

        AnimationController.Instance.AddAnimation(AnimateFieldToHand(res));
    }

    private IEnumerator AnimateFieldToHand(ResponseFieldToHandEventArgs res)
    {
        bool isLocal = (res.CasterId == Constants.USER_ID);
        GameObject cardGO = null;

        // 1. Find the card on the field (most recently added)
        if (cardsOnField.Count > 0)
        {
            cardGO = cardsOnField[cardsOnField.Count - 1];
            cardsOnField.RemoveAt(cardsOnField.Count - 1);
        }

        if (cardGO == null) yield break;

        // 2. Hide judge indicator
        var ui = cardGO.GetComponent<CardUIController>();
        if (ui != null) ui.HideJudgementResult();

        // 3. Animate to Hand
        Vector3 targetPos = Vector3.down * 500;
        if (isLocal)
        {
            // Position in local hand
            targetPos = handManager.GetPredictiveWorldPosition(handManager.GetCardIndex(-1) + 1, GameSession.Instance.GetLocalPlayer().Hand.Count + 1);
        }
        else
        {
            // Position of caster's champion
            if (GameSession.Instance.Players.TryGetValue(res.CasterId, out PlayerData pData) && pData.ChampionObject != null)
            {
                targetPos = pData.ChampionObject.transform.position;
            }
        }

        yield return SmoothMove(cardGO.transform, targetPos, Quaternion.identity, playAnimDuration);

        // 4. Register and Cleanup
        if (isLocal)
        {
            CardData data = new CardData { Id = res.CardId, Suit = res.Suit, Value = res.Value, Type = res.CardType };
            handManager.RegisterAnimatedCard(data, cardGO);
            GameSession.Instance.GetLocalPlayer().AddCard(data);
        }
        else
        {
            Destroy(cardGO);
            if (GameSession.Instance.Players.TryGetValue(res.CasterId, out PlayerData pData))
            {
                pData.AddCard(new CardData { Id = -1 });
            }
        }
    }

    private void OnResponseDiscardCard(ExtendedEventArgs args)
    {
        ResponseDiscardCardEventArgs res = args as ResponseDiscardCardEventArgs;
        if (res == null) return;

        AnimationController.Instance.AddAnimation(AnimateDiscardCard(res));
    }

    private IEnumerator AnimateDiscardCard(ResponseDiscardCardEventArgs res)
    {
        GameObject cardGO = null;
        bool isLocal = (res.PlayerId == Constants.USER_ID);

        if (isLocal)
        {
            cardGO = handManager.GetCardObject(res.CardId);
            if (cardGO != null)
            {
                handManager.UnregisterCard(res.CardId);
                cardGO.transform.SetParent(playFieldPanel, true);
            }
        }

        // Update counts
        if (GameSession.Instance.Players.TryGetValue(res.PlayerId, out PlayerData pData))
        {
            if (isLocal) pData.RemoveCardById(res.CardId);
            else pData.RemoveCardById(-1);
        }

        if (cardGO == null)
        {
            cardGO = Instantiate(cardPrefab, playFieldPanel);
            CardSetup setup = cardGO.GetComponent<CardSetup>();
            if (setup != null) setup.Init(res.CardType, res.Suit, res.Value);

            if (pData != null && pData.ChampionObject != null)
                cardGO.transform.position = pData.ChampionObject.transform.position;
        }

        // Show Judge result if requested
        if (res.ShowJudge)
        {
            var ui = cardGO.GetComponent<CardUIController>();
            if (ui != null) ui.ShowJudgementResult(res.JudgeResult);
        }

        cardsOnField.Add(cardGO);
        ReorganizeField();

        yield return new WaitForSeconds(playAnimDuration);
        
        // Note: No Audio for Discard as requested.
    }

    private void OnResponseSwapFieldHand(ExtendedEventArgs args)
    {
        ResponseSwapFieldHandEventArgs res = args as ResponseSwapFieldHandEventArgs;
        if (res == null) return;

        AnimationController.Instance.AddAnimation(AnimateSwapFieldHand(res));
    }

    private IEnumerator AnimateSwapFieldHand(ResponseSwapFieldHandEventArgs res)
    {
        bool isLocal = (res.CasterId == Constants.USER_ID);
        GameObject fieldCardGO = null;

        // 1. Identify the card currently on the field (the one being swapped back to hand)
        // Note: For simplicity, assuming the target card is the most recently added to cardsOnField
        if (cardsOnField.Count > 0)
        {
            fieldCardGO = cardsOnField[cardsOnField.Count - 1];
            cardsOnField.RemoveAt(cardsOnField.Count - 1);
        }

        if (fieldCardGO != null)
        {
            // Disable its judge indicator immediately
            var ui = fieldCardGO.GetComponent<CardUIController>();
            if (ui != null) ui.HideJudgementResult();
        }

        // 2. Prepare the card coming FROM hand
        GameObject handCardGO = null;
        int originalIndex = -1; // --- NEW: Track original position ---

        if (isLocal)
        {
            handCardGO = handManager.GetCardObject(res.PlayedCardId);
            if (handCardGO != null)
            {
                originalIndex = handManager.GetCardIndex(res.PlayedCardId); // --- NEW: Save index ---
                handManager.UnregisterCard(res.PlayedCardId);
                handCardGO.transform.SetParent(playFieldPanel, true);
            }
        }

        if (handCardGO == null)
        {
            // Non-local or not found: Spawn at caster's location
            handCardGO = Instantiate(cardPrefab, playFieldPanel);
            CardSetup setup = handCardGO.GetComponent<CardSetup>();
            if (setup != null) setup.Init(res.PlayedCardType, res.PlayedSuit, res.PlayedValue);

            if (GameSession.Instance.Players.TryGetValue(res.CasterId, out PlayerData casterData) && casterData.ChampionObject != null)
            {
                handCardGO.transform.position = casterData.ChampionObject.transform.position;
            }
        }

        // --- NEW: Enable judgement image on the card coming FROM hand immediately ---
        var newUI = handCardGO.GetComponent<CardUIController>();
        if (newUI != null)
        {
            newUI.ShowJudgementResult(res.JudgeResult);
        }

        // 3. Animate the swap
        // Card A: Field -> Hand (or destroy)
        Vector3 handPos = Vector3.down * 500; // General direction of hand
        if (isLocal && handManager != null)
        {
            // For local, we could use handManager to get a real position, but simplified for now
        }

        if (fieldCardGO != null)
        {
            StartCoroutine(SmoothMove(fieldCardGO.transform, handPos, Quaternion.identity, playAnimDuration));
        }

        // Card B: Hand -> Field
        cardsOnField.Add(handCardGO);
        ReorganizeField(); // This handles the move to playField center

        yield return new WaitForSeconds(playAnimDuration);

        // 4. Cleanup and Finalize
        if (fieldCardGO != null)
        {
            if (isLocal)
            {
                // Re-register to local hand AT THE SAME POSITION
                CardData newCard = new CardData { Id = res.SwappedCardId, Suit = res.SwappedSuit, Value = res.SwappedValue, Type = res.SwappedCardType };
                
                // Give the object to HandManager
                handManager.RegisterAnimatedCardAtIndex(newCard, fieldCardGO, originalIndex);
                
                // Update local data model
                GameSession.Instance.GetLocalPlayer().AddCard(newCard);
            }
            else
            {
                Destroy(fieldCardGO);
            }
        }

        if (newUI != null) newUI.ShowJudgementResult(res.JudgeResult);

        // Update Data counts
        if (GameSession.Instance.Players.TryGetValue(res.CasterId, out PlayerData pData))
        {
            if (isLocal)
            {
                pData.RemoveCardById(res.PlayedCardId);
                // SwappedCardId was added via handManager.AddCard above
            }
            else
            {
                // Non-local counts: card played from hand, card received to hand. Net change 0.
            }
        }
    }

    private void OnJudgementResult(ExtendedEventArgs args)
    {
        ResponseJudgementEventArgs res = args as ResponseJudgementEventArgs;
        if (res == null) return;

        AnimationController.Instance.AddAnimation(AnimateJudgement(res));
    }

    private IEnumerator AnimateJudgement(ResponseJudgementEventArgs res)
    {
        // 1. Instantiate judge card
        GameObject cardGO = Instantiate(cardPrefab, playFieldPanel);
        CardSetup setup = cardGO.GetComponent<CardSetup>();
        if (setup != null)
        {
            setup.Init(res.CardType, res.Suit, res.Value);
        }

        // Start from center/top
        cardGO.transform.localPosition = new Vector3(0, 500, 0);

        // 2. Add to field and reorganize
        cardsOnField.Add(cardGO);
        ReorganizeField();

        yield return new WaitForSeconds(playAnimDuration);

        // 3. Show current evaluation result on the card
        CardUIController uiController = cardGO.GetComponent<CardUIController>();
        if (uiController != null)
        {
            uiController.ShowJudgementResult(res.JudgeResult);
        }

        yield return new WaitForSeconds(1.5f); // Duration to show the result
    }

    private void OnResponsePlayCard(ExtendedEventArgs args)
    {
        ResponsePlayCardEventArgs res = args as ResponsePlayCardEventArgs;
        if (res == null) return;

        // Add to animation queue to ensure sequence
        AnimationController.Instance.AddAnimation(AnimateCardPlay(res));
    }

    private IEnumerator AnimateCardPlay(ResponsePlayCardEventArgs res)
    {
        GameObject cardGO = null;
        bool isLocal = (res.PlayerId == Constants.USER_ID);

        if (isLocal)
        {
            // 1. Local Player: Find the card in HandManager and "take" it
            cardGO = handManager.GetCardObject(res.CardId);
            if (cardGO != null)
            {
                // Unregister from hand so it doesn't get reorganized
                handManager.UnregisterCard(res.CardId);
                // Move to play field parent but keep world position for now
                cardGO.transform.SetParent(playFieldPanel, true);
            }
        }

        // --- NEW: Update PlayerData Hand Count ---
        if (GameSession.Instance.Players.TryGetValue(res.PlayerId, out PlayerData pData))
        {
            if (isLocal)
                pData.RemoveCardById(res.CardId);
            else
                pData.RemoveCardById(-1);
        }
        // ----------------------------------------
        
        // 2. If card wasn't found (non-local or error), create a new one
        if (cardGO == null)
        {
            cardGO = Instantiate(cardPrefab, playFieldPanel);
            CardSetup setup = cardGO.GetComponent<CardSetup>();
            if (setup != null)
            {
                setup.Init(res.CardType, res.Suit, res.Value);
            }

            // Start position for non-local: the player's character
            if (GameSession.Instance.Players.TryGetValue(res.PlayerId, out pData) && pData.ChampionObject != null)
            {
                // Convert 3D world position to canvas local position
                Vector3 worldPos = pData.ChampionObject.transform.position;
                cardGO.transform.position = worldPos; // Direct world position works for WorldSpace Canvas
            }
            else
            {
                // Fallback to center if no character found
                cardGO.transform.localPosition = Vector3.zero;
            }
        }

        // 3. Record the card on the field BEFORE moving so it's included in reorganization
        cardsOnField.Add(cardGO);

        // --- NEW: Enable judgement image before animation if requested ---
        if (res.ShowJudge)
        {
            CardUIController uiController = cardGO.GetComponent<CardUIController>();
            if (uiController != null)
            {
                // Use the actual result passed in the packet
                uiController.ShowJudgementResult(res.JudgeResult); 
            }
        }

        // 4. Reorganize everything on the field
        ReorganizeField();

        // Wait for the duration of the play animation
        yield return new WaitForSeconds(playAnimDuration);
    }

    private IEnumerator SmoothMove(Transform tr, Vector3 targetLocalPos, Quaternion targetLocalRot, float duration, GameObject owner = null)
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

        if (owner != null) activeCoroutines.Remove(owner);
    }
}
