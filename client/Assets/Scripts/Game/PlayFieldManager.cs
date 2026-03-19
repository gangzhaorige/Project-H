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
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_PLAY_CARD);
        }
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
