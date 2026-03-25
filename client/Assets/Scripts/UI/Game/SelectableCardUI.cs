using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProjectH.Models;

public class SelectableCardUI : MonoBehaviour, IPointerClickHandler
{
    private int cardId;
    private int index;
    private bool isSelected = false;
    
    [SerializeField] private Image selectionHighlight;
    private SelectPanelManager manager;

    public void Init(int cardId, int index, SelectPanelManager manager)
    {
        this.cardId = cardId;
        this.index = index;
        this.manager = manager;

        selectionHighlight.gameObject.SetActive(true);

        if (selectionHighlight != null) selectionHighlight.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[SelectableCardUI] Clicked card index: {index}, current selection state: {isSelected}");
        // Don't allow selection if we're already at the limit
        if (!isSelected && !manager.CanSelectMore()) return;

        isSelected = !isSelected;
        if (selectionHighlight != null) selectionHighlight.enabled = isSelected;
        manager.OnCardSelected(index, isSelected);
    }
}
