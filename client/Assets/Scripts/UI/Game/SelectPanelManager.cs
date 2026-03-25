using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ProjectH.Models;

public class SelectPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI messageText;

    private int requiredAmount;
    private List<int> selectedIndices = new List<int>();
    private float timer;
    private bool isActive;

    private void OnEnable()
    {
        GameSession.Instance.OnSelectCardsFromOpponent += OnSelectionRequested;
        GameSession.Instance.OnSelectCardsCompleted += ClosePanel;
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirm);
    }

    private void OnDisable()
    {
        GameSession.Instance.OnSelectCardsFromOpponent -= OnSelectionRequested;
        GameSession.Instance.OnSelectCardsCompleted -= ClosePanel;
        if (confirmButton != null) confirmButton.onClick.RemoveListener(OnConfirm);
    }

    private void Update()
    {
        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) ClosePanel();
        }
    }

    private void OnSelectionRequested(int targetPlayerId, int amount, int duration, string message, int targetHandSize)
    {
        Debug.Log($"[SelectPanelManager] Selection requested from player {targetPlayerId}. HandSize: {targetHandSize}");
        if (selectPanel != null) 
        {
            selectPanel.SetActive(true);
            Debug.Log("[SelectPanelManager] SelectPanel activated.");
        }
        else
        {
            Debug.LogError("[SelectPanelManager] selectPanel reference is NULL!");
        }
        requiredAmount = amount;
        timer = duration;
        // messageText.text = message;
        selectedIndices.Clear();
        isActive = true;

        // Clear container
        foreach (Transform child in cardsContainer) Destroy(child.gameObject);

        // Fetch target player hand
        if (GameSession.Instance.Players.TryGetValue(targetPlayerId, out PlayerData targetPlayer))
        {
            // Use targetHandSize to display the correct number of cards/placeholders
            for (int i = 0; i < targetHandSize; i++)
            {
                // If we have local card data, pass it; otherwise it's just a placeholder
                int cardId = (i < targetPlayer.Hand.Count) ? targetPlayer.Hand[i].Id : -1;
                
                GameObject go = Instantiate(cardPrefab, cardsContainer);
                go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                
                // Disable the default CardUIController so it doesn't interfere with the selection panel logic
                var uiController = go.GetComponent<CardUIController>();
                if (uiController != null) uiController.enabled = false;

                var selectable = go.GetComponent<SelectableCardUI>();
                if (selectable == null)
                {
                    Debug.Log("[SelectPanelManager] SelectableCardUI missing from prefab, adding it dynamically.");
                    selectable = go.AddComponent<SelectableCardUI>();
                }

                if (selectable != null)
                {
                    selectable.enabled = true; // Ensure the selection script is active
                    selectable.Init(cardId, i, this);
                }
            }
        }

        confirmButton.interactable = false;
    }

    public bool CanSelectMore()
    {
        return selectedIndices.Count < requiredAmount;
    }

    public void OnCardSelected(int index, bool selected)
    {
        if (selected)
        {
            if (!selectedIndices.Contains(index)) selectedIndices.Add(index);
        }
        else
        {
            selectedIndices.Remove(index);
        }
        bool ready = (selectedIndices.Count == requiredAmount);
        Debug.Log($"[SelectPanelManager] Card {index} {(selected ? "selected" : "deselected")}. Total selected: {selectedIndices.Count}/{requiredAmount}. Ready: {ready}");
        confirmButton.interactable = ready;
    }

    public void OnConfirm()
    {
        Debug.Log($"[SelectPanelManager] OnConfirm clicked. Selected count: {selectedIndices.Count}/{requiredAmount}");
        if (selectedIndices.Count != requiredAmount)
        {
            Debug.LogWarning("[SelectPanelManager] Cannot confirm: incorrect number of cards selected.");
            return;
        }

        Debug.Log($"[SelectPanelManager] Sending RequestSelectCards with indices: {string.Join(",", selectedIndices)}");
        
        RequestSelectCards req = new RequestSelectCards();
        req.Send(selectedIndices);
        NetworkManager.Instance.SendRequest(req);

        // Do not close panel immediately; wait for server confirmation (SMSG_SELECT_CARDS)
        confirmButton.interactable = false;
    }

    private void ClosePanel()
    {
        isActive = false;
        selectPanel.SetActive(false);
    }
}
