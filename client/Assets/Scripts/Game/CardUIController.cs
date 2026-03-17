using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProjectH.Models;

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
        if (playButton != null) playButton.SetActive(true);
        
        // Target the hover offset
        targetYOffset = hoverYOffset;

        if (handManager != null && cardData != null)
        {
            handManager.SetHoveredCard(cardData.Id);
        }
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
        Debug.Log($"[CardUI] INTERNAL: OnPlayButtonClick triggered for card {(cardData != null ? cardData.Id.ToString() : "NULL")}");
        
        if (cardData == null)
        {
            Debug.LogError("[CardUI] Cannot play: cardData is null!");
            return;
        }

        if (CardTargetSelector.Instance == null)
        {
            Debug.LogError("[CardUI] Cannot play: CardTargetSelector.Instance is missing in the scene!");
            return;
        }

        CardTargetSelector.Instance.BeginTargeting(cardData);
    }
}
