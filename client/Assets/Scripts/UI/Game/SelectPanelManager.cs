using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ProjectH.Models;
using ProjectH.UI;

/**
 * Manages the logic for the card selection panel (e.g., stealing cards from an opponent).
 */
public class SelectPanelManager : MonoBehaviour
{
    private SelectPanelView _view;
    
    private int requiredAmount;
    private List<int> selectedIndices = new List<int>();
    private float timer;
    private bool isActive;

    public void Init(SelectPanelView view)
    {
        _view = view;
        if (_view != null && _view.confirmButton != null)
        {
            _view.confirmButton.onClick.AddListener(OnConfirm);
        }
    }

    private void OnDestroy()
    {
        if (_view != null && _view.confirmButton != null)
        {
            _view.confirmButton.onClick.RemoveListener(OnConfirm);
        }
    }

    private void OnEnable()
    {
        GameSession.Instance.OnSelectCardsFromOpponent += OnSelectionRequested;
        GameSession.Instance.OnSelectCardsCompleted += ClosePanel;
    }

    private void OnDisable()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.OnSelectCardsFromOpponent -= OnSelectionRequested;
            GameSession.Instance.OnSelectCardsCompleted -= ClosePanel;
        }
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
        
        if (_view == null)
        {
            Debug.LogError("[SelectPanelManager] View reference is NULL!");
            return;
        }

        _view.gameObject.SetActive(true);
        Debug.Log("[SelectPanelManager] SelectPanel activated.");

        requiredAmount = amount;
        timer = duration;
        
        if (_view.messageText != null)
        {
            _view.messageText.text = message;
        }

        selectedIndices.Clear();
        isActive = true;

        // Clear container
        foreach (Transform child in _view.cardsContainer) Destroy(child.gameObject);

        // Fetch target player hand
        if (GameSession.Instance.Players.TryGetValue(targetPlayerId, out PlayerData targetPlayer))
        {
            for (int i = 0; i < targetHandSize; i++)
            {
                int cardId = (i < targetPlayer.Hand.Count) ? targetPlayer.Hand[i].Id : -1;
                
                GameObject go = Instantiate(_view.cardPrefab, _view.cardsContainer);
                go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                
                var uiController = go.GetComponent<CardUIController>();
                if (uiController != null) uiController.enabled = false;

                var selectable = go.GetComponent<SelectableCardUI>();
                if (selectable == null)
                {
                    selectable = go.AddComponent<SelectableCardUI>();
                }

                if (selectable != null)
                {
                    selectable.enabled = true;
                    selectable.Init(cardId, i, this);
                }
            }
        }

        if (_view.confirmButton != null)
        {
            _view.confirmButton.interactable = false;
        }
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
        
        if (_view != null && _view.confirmButton != null)
        {
            _view.confirmButton.interactable = ready;
        }
    }

    public void OnConfirm()
    {
        if (selectedIndices.Count != requiredAmount) return;

        RequestSelectCards req = new RequestSelectCards();
        req.Send(selectedIndices);
        NetworkManager.Instance.SendRequest(req);

        if (_view != null && _view.confirmButton != null)
        {
            _view.confirmButton.interactable = false;
        }
    }

    private void ClosePanel()
    {
        isActive = false;
        if (_view != null)
        {
            _view.gameObject.SetActive(false);
        }
    }
}
