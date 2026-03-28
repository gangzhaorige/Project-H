using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ProjectH.Models;
using ProjectH.UI;

/**
 * Manages the logic for selecting targets for cards or skills.
 */
public class TargetSelectionManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject targetItemPrefab;

    private List<int> selectedTargetIds = new List<int>();
    private List<TargetItemUI> activeItems = new List<TargetItemUI>();
    
    private CardData currentCard;
    private int currentSkillId = -1;
    private int maxTargetsForSkill = 0;

    private UIController _uiController;
    private CardTargetSelector _cardTargetSelector;
    private TargetSelectionPanelView _panel;

    public void Init(UIController uiController, CardTargetSelector cardTargetSelector, TargetSelectionPanelView panel)
    {
        _uiController = uiController;
        _cardTargetSelector = cardTargetSelector;
        _panel = panel;

        if (_panel != null)
        {
            if (_panel.confirmButton != null) _panel.confirmButton.onClick.AddListener(OnConfirmClick);
            if (_panel.cancelButton != null) _panel.cancelButton.onClick.AddListener(OnCancelClick);
        }
    }

    private void OnDestroy()
    {
        if (_panel != null)
        {
            if (_panel.confirmButton != null) _panel.confirmButton.onClick.RemoveListener(OnConfirmClick);
            if (_panel.cancelButton != null) _panel.cancelButton.onClick.RemoveListener(OnCancelClick);
        }
    }

    public void Show(CardData card)
    {
        currentCard = card;
        currentSkillId = -1;
        selectedTargetIds.Clear();
        
        if (_panel != null && _panel.instructionText != null)
        {
            _panel.instructionText.text = $"Select targets for {GetCardTypeName(card.Type)} (Max: {(_cardTargetSelector != null ? _cardTargetSelector.GetMaxTargets(card.Type) : 0)})";
        }

        RefreshList();
        UpdateConfirmButton();
    }

    private string GetCardTypeName(int type)
    {
        if (System.Enum.IsDefined(typeof(CardData.NormalType), type))
            return ((CardData.NormalType)type).ToString();
        if (System.Enum.IsDefined(typeof(CardData.SpecialType), type))
            return ((CardData.SpecialType)type).ToString();
        return "Unknown";
    }

    public void ShowForSkill(int skillId, int maxTargets)
    {
        currentSkillId = skillId;
        currentCard = null;
        maxTargetsForSkill = maxTargets;
        selectedTargetIds.Clear();

        if (_panel != null && _panel.instructionText != null)
        {
            _panel.instructionText.text = $"Select targets for Skill {skillId} (Max: {maxTargets})";
        }

        RefreshList();
        UpdateConfirmButton();
    }

    private void RefreshList()
    {
        if (_panel == null || _panel.container == null) return;

        // Clear existing
        foreach (var item in activeItems) if (item != null) Destroy(item.gameObject);
        activeItems.Clear();

        foreach (var pair in GameSession.Instance.Players)
        {
            PlayerData pData = pair.Value;
            
            if (pData.PlayerId == Constants.USER_ID) continue;

            bool inRange = true;
            if (currentCard != null && currentCard.Type == (int)CardData.NormalType.ATTACK)
            {
                inRange = _cardTargetSelector != null && _cardTargetSelector.CanTarget(Constants.USER_ID, pData.PlayerId);
                
                if (!inRange) continue;
            }

            GameObject go = Instantiate(targetItemPrefab, _panel.container);
            TargetItemUI itemUI = go.GetComponent<TargetItemUI>();
            
            itemUI.Setup(pData, inRange, OnTargetToggled);
            activeItems.Add(itemUI);
        }
    }

    private void OnTargetToggled(int playerId, bool isSelected)
    {
        if (isSelected)
        {
            if (!selectedTargetIds.Contains(playerId))
            {
                int max = (currentCard != null) ? (_cardTargetSelector != null ? _cardTargetSelector.GetMaxTargets(currentCard.Type) : 0) : maxTargetsForSkill;
                
                if (selectedTargetIds.Count >= max)
                {
                    int first = selectedTargetIds[0];
                    selectedTargetIds.RemoveAt(0);
                    foreach(var item in activeItems) if(item.PlayerId == first) item.SetSelected(false);
                }
                selectedTargetIds.Add(playerId);
            }
        }
        else
        {
            selectedTargetIds.Remove(playerId);
        }

        UpdateConfirmButton();
    }

    private void UpdateConfirmButton()
    {
        if (_panel != null && _panel.confirmButton != null)
        {
            if (currentCard != null)
            {
                _panel.confirmButton.interactable = selectedTargetIds.Count > 0 || (currentCard.Type == (int)CardData.NormalType.HEAL && selectedTargetIds.Count == 0);
            }
            else if (currentSkillId != -1)
            {
                _panel.confirmButton.interactable = selectedTargetIds.Count > 0;
            }
        }
    }

    private void OnConfirmClick()
    {
        if (_cardTargetSelector != null) _cardTargetSelector.ConfirmTargeting(selectedTargetIds);
        if (_uiController != null) _uiController.ShowTargetSelectionPanel(false);
    }

    private void OnCancelClick()
    {
        if (_uiController != null) _uiController.ShowTargetSelectionPanel(false);
        if (_cardTargetSelector != null) _cardTargetSelector.CancelTargeting();
    }
}
