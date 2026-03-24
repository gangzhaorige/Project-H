using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProjectH.Models;
using ProjectH.Rules;

public class CardUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI References")]
    public RectTransform cardVisualContainer;
    public Image judgeResultImage;

    [Header("Settings")]
    public float selectedYOffset = 80f;
    public float transitionSpeed = 10f;

    private CardData cardData;
    private HandManager handManager;
    private float targetYOffset = 0f;

    public void Bind(CardData data, HandManager handManager)
    {
        this.cardData = data;
        this.handManager = handManager;

        Debug.Log($"[CardUI] Bound card {data.Id} ({data.Type}) to UI.");
        HideJudgementResult();
        
        targetYOffset = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hover effects removed
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hover effects removed
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[CardUI] Clicked card: {cardData?.Type}");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCardHoverSFX(); // Play sound on click now
        }
        
        if (handManager == null) 
        {
            Debug.LogError("[CardUI] Cannot click: HandManager is null!");
            return;
        }

        if (cardData == null) return;

        // Check if the card can even be played right now before allowing selection
        if (!CanSelectCard())
        {
            Debug.LogWarning("[CardUI] Cannot select card: Not your turn or no response required.");
            return;
        }

        handManager.ToggleCardSelection(cardData.Id);
        Debug.Log($"[CardUI] Toggled selection for {cardData.Type}. Is Selected: {handManager.IsSelected(cardData.Id)}");
    }

    private bool CanSelectCard()
    {
        if (cardData == null) return false;

        // Allow playing any card during SkillResolutionState or Response Required
        if (GameSession.Instance.State == "SkillResolutionState" || GameSession.Instance.IsResponseRequired) return true;

        var localPlayer = GameSession.Instance.GetLocalPlayer();
        return CardRuleManager.CanPlay(cardData, localPlayer);
    }

    private void Update()
    {
        // Determine target Y based on Selection state
        targetYOffset = 0f;
        if (handManager != null && cardData != null)
        {
            bool isSelected = handManager.IsSelected(cardData.Id);
            
            if (isSelected) 
            {
                targetYOffset = selectedYOffset;
            }
        }

        // Smoothly slide the card up/down on the Y axis ONLY
        if (cardVisualContainer != null)
        {
            Vector3 currentPos = cardVisualContainer.localPosition;
            float newY = Mathf.Lerp(currentPos.y, targetYOffset, Time.deltaTime * transitionSpeed);
            
            // PRESERVE X and Z to avoid stacking issues if cardVisualContainer is the root
            cardVisualContainer.localPosition = new Vector3(currentPos.x, newY, currentPos.z);
        }
    }

    public void ShowJudgementResult(bool approved)
    {
        if (judgeResultImage == null) return;

        string resultName = approved ? "1" : "0";
        Sprite s = Resources.Load<Sprite>($"Images/cards/Judge/{resultName}");
        if (s != null)
        {
            judgeResultImage.sprite = s;
            judgeResultImage.gameObject.SetActive(true);
        }
    }

    public void HideJudgementResult()
    {
        if (judgeResultImage != null)
        {
            judgeResultImage.gameObject.SetActive(false);
        }
    }
}
