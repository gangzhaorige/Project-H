using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class HandManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("UI Containers")]
    public RectTransform handPanel;

    private Dictionary<int, GameObject> cardObjects = new Dictionary<int, GameObject>();

    /// <summary>
    /// Adds a list of cards to the hand.
    /// </summary>
    public void AddCards(List<CardData> newCards)
    {
        foreach (var card in newCards)
        {
            AddCard(card);
        }
    }

    /// <summary>
    /// Adds a single card to the hand UI.
    /// </summary>
    public void AddCard(CardData data)
    {
        if (cardObjects.ContainsKey(data.Id)) return;

        GameObject cardGO = Instantiate(cardPrefab, handPanel);
        cardGO.name = $"Card_{data.Type}_{data.Id}";
        
        CardSetup setup = cardGO.GetComponent<CardSetup>();
        if (setup != null)
        {
            setup.Init(data.Type, data.Suit, data.Value);
        }

        cardObjects.Add(data.Id, cardGO);
    }

    /// <summary>
    /// Removes a card from the hand UI by its ID.
    /// </summary>
    public void RemoveCard(int cardId)
    {
        if (cardObjects.ContainsKey(cardId))
        {
            Destroy(cardObjects[cardId]);
            cardObjects.Remove(cardId);
        }
    }

    /// <summary>
    /// Clears all cards from the hand UI.
    /// </summary>
    public void ClearHand()
    {
        foreach (var card in cardObjects.Values)
        {
            Destroy(card);
        }
        cardObjects.Clear();
    }
}
