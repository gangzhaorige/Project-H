using System.Collections.Generic;
using UnityEngine;

namespace ProjectH.Models
{
    [System.Serializable]
    public class ChampionData
    {
        public int Id;
        public string Name;
        public int MaxHP;
        public int CurHP;
        public int PathId;
        public string Element;
        public int Attack;
        public int AttackRange;
        public int SpecialDefense;
        // Other stats can be added here
    }

    [System.Serializable]
    public class PlayerData
    {
        public int PlayerId;
        public string Username;
        public int Team;
        public ChampionData Champion;
        public GameObject ChampionObject; // Reference to the spawned prefab
        public List<CardData> Hand = new List<CardData>();

        public delegate void HandChanged();
        public event HandChanged OnHandChanged;

        public void AddCard(CardData card)
        {
            Hand.Add(card);
            OnHandChanged?.Invoke();
        }

        public void RemoveCard(CardData card)
        {
            Hand.Remove(card);
            OnHandChanged?.Invoke();
        }

        public void RemoveCardById(int cardId)
        {
            CardData card = Hand.Find(c => c.Id == cardId);
            if (card != null)
            {
                Hand.Remove(card);
                OnHandChanged?.Invoke();
            }
        }

        public void ClearHand()
        {
            Hand.Clear();
            OnHandChanged?.Invoke();
        }
    }

    public class GameSession
    {
        private static GameSession _instance;
        public static GameSession Instance => _instance ??= new GameSession();

        public Dictionary<int, PlayerData> Players = new Dictionary<int, PlayerData>();
        public List<int> PlayerOrder = new List<int>();

        public string State = "";

        public void Clear()
        {
            Players.Clear();
            PlayerOrder.Clear();
        }

        public PlayerData GetLocalPlayer()
        {
            if (Players.ContainsKey(Constants.USER_ID))
                return Players[Constants.USER_ID];
            return null;
        }
    }
}
