using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProjectH.Models;
using ProjectH.Rules;

public class CardUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public GameObject playButton;
    public RectTransform cardVisualContainer; // The part that should pop up on hover

    [Header("Settings")]
    public float hoverYOffset = 50f;
    public float transitionSpeed = 10f;

    private CardData cardData;
    private HandManager handManager;
    private bool isHovered = false;
    private float targetYOffset = 0f;

    public void Bind(CardData data)
    {
        this.cardData = data;
        this.handManager = GetComponentInParent<HandManager>();
        
        Debug.Log($"[CardUI] Bound card {data.Id} ({data.Type}) to UI.");
        if (playButton != null) playButton.SetActive(false);
        
        targetYOffset = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (playButton != null) playButton.SetActive(CanShowPlayButton());
        
        // Target the hover offset
        targetYOffset = hoverYOffset;

        if (handManager != null && cardData != null)
        {
            handManager.SetHoveredCard(cardData.Id);
        }
    }

    private bool CanShowPlayButton()
    {
        if (cardData == null) return false;

        var localPlayer = GameSession.Instance.GetLocalPlayer();
        return CardRuleManager.CanPlay(cardData, localPlayer);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (playButton != null) playButton.SetActive(false);
        
        // Reset the hover offset
        targetYOffset = 0f;

        if (handManager != null && cardData != null)
        {
            handManager.ClearHoveredCard(cardData.Id);
        }
    }

    private void Update()
    {
        // Smoothly slide the card up/down on the Y axis ONLY
        if (cardVisualContainer != null)
        {
            Vector3 currentPos = cardVisualContainer.localPosition;
            float newY = Mathf.Lerp(currentPos.y, targetYOffset, Time.deltaTime * transitionSpeed);
            
            // PRESERVE X and Z to avoid stacking issues if cardVisualContainer is the root
            cardVisualContainer.localPosition = new Vector3(currentPos.x, newY, currentPos.z);
        }
    }

    public void OnPlayButtonClick()
    {
        Debug.Log($"[CardUI] INTERNAL: OnPlayButtonClick for card {(cardData != null ? cardData.Id.ToString() : "NULL")}");
        
        if (cardData == null) return;

        // 1. During PlayActionState: Allow playing any card with its standard requirements/targeting.
        if (GameSession.Instance.State == "PlayActionState")
        {
            HandleStandardPlay();
            return;
        }

        // 2. Response Phase (Not PlayActionState): Play required card automatically without targeting.
        if (GameSession.Instance.IsResponseRequired)
        {
            if (cardData.Type == GameSession.Instance.RequiredCardType)
            {
                Debug.Log($"[CardUI] Playing required response card {cardData.Type} automatically.");
                SendPlayRequest(new System.Collections.Generic.List<int>());
            }
            else
            {
                Debug.LogWarning($"[CardUI] Card type mismatch. Required: {GameSession.Instance.RequiredCardType}, Played: {cardData.Type}");
            }
            return;
        }

        Debug.LogWarning("[CardUI] Cannot play card: Not your turn or no response required.");
    }

    private void HandleStandardPlay()
    {
        // Validate targeting requirement
        int maxTargets = CardRuleManager.GetMaxTargets(cardData);
        if (maxTargets > 0)
        {
            Debug.Log($"[CardUI] Card {cardData.Type} requires {maxTargets} targets. Showing UI.");
            CardTargetSelector.Instance.BeginTargeting(cardData);
        }
        else
        {
            Debug.Log($"[CardUI] Card {cardData.Type} requires no targets. Playing directly.");
            SendPlayRequest(new System.Collections.Generic.List<int>());
        }
    }

    private void SendPlayRequest(System.Collections.Generic.List<int> targetIds)
    {
        RequestPlayCard req = new RequestPlayCard();
        req.Send(cardData.Id, targetIds);
        NetworkManager.Instance.SendRequest(req);

        // Immediately update UI state to avoid double-clicking or invalid actions
        if (UIController.Instance != null) UIController.Instance.UpdateUIState();
    }
}
