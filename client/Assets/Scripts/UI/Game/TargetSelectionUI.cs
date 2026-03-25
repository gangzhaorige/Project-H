using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ProjectH.Models;

public class TargetSelectionUI : MonoBehaviour
{
    public static TargetSelectionUI Instance { get; private set; }

    [Header("UI References")]
    public Transform container;
    public GameObject targetItemPrefab;
    public Button confirmButton;
    public Button cancelButton;
    public TextMeshProUGUI instructionText;

    private CardData currentCard;
    private int currentSkillId = -1;
    private int maxTargetsForSkill = 0;
    private List<int> selectedTargetIds = new List<int>();
    private List<TargetItemUI> activeItems = new List<TargetItemUI>();

    private void Awake()
    {
        Instance = this;
        
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClick);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelClick);
    }

    public void Show(CardData card)
    {
        currentCard = card;
        currentSkillId = -1;
        selectedTargetIds.Clear();
        
        if (instructionText != null)
        {
            instructionText.text = $"Select targets for {card.Type} (Max: {CardTargetSelector.Instance.GetMaxTargets(card.Type)})";
        }

        RefreshList();
        UpdateConfirmButton();
    }

    public void ShowForSkill(int skillId, int maxTargets)
    {
        currentSkillId = skillId;
        currentCard = null;
        maxTargetsForSkill = maxTargets;
        selectedTargetIds.Clear();

        if (instructionText != null)
        {
            instructionText.text = $"Select targets for Skill {skillId} (Max: {maxTargets})";
        }

        RefreshList();
        UpdateConfirmButton();
    }

    private void RefreshList()
    {
        // Clear existing
        foreach (var item in activeItems) Destroy(item.gameObject);
        activeItems.Clear();

        foreach (var pair in GameSession.Instance.Players)
        {
            PlayerData pData = pair.Value;
            
            // If the card requires targets (MaxTargets > 0), we assume it targets others.
            // (Self-targeting cards like HEAL should have MaxTargets == 0 and bypass this UI).
            if (pData.PlayerId == Constants.USER_ID) continue;

            bool inRange = true;
            if (currentCard != null && currentCard.Type == "ATTACK")
            {
                inRange = CardTargetSelector.Instance.CanTarget(Constants.USER_ID, pData.PlayerId);
                
                // If out of range for Attack card, do not show at all
                if (!inRange) continue;
            }

            GameObject go = Instantiate(targetItemPrefab, container);
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
                int max = (currentCard != null) ? CardTargetSelector.Instance.GetMaxTargets(currentCard.Type) : maxTargetsForSkill;
                // Enforce max targets
                if (selectedTargetIds.Count >= max)
                {
                    // Deselect the oldest one
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
        if (confirmButton != null)
        {
            if (currentCard != null)
            {
                // Allow confirm if at least one target is selected (or 0 if card allows it, but usually 1+)
                confirmButton.interactable = selectedTargetIds.Count > 0 || (currentCard.Type == "HEAL" && selectedTargetIds.Count == 0);
            }
            else if (currentSkillId != -1)
            {
                confirmButton.interactable = selectedTargetIds.Count > 0;
            }
        }
    }

    private void OnConfirmClick()
    {
        CardTargetSelector.Instance.ConfirmTargeting(selectedTargetIds);
        if (UIController.Instance != null) UIController.Instance.ShowTargetSelectionPanel(false);
    }

    private void OnCancelClick()
    {
        if (UIController.Instance != null) UIController.Instance.ShowTargetSelectionPanel(false);
        CardTargetSelector.Instance.CancelTargeting();
    }
}
