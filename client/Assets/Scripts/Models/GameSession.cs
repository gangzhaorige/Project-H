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
        public int Element;
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

        private string _state = "";
        public string State 
        { 
            get => _state; 
            set 
            { 
                if (_state != value) {
                    _state = value; 
                    OnStateChanged?.Invoke(_state);
                }
            }
        }

        private bool _isResponseRequired = false;
        public bool IsResponseRequired
        {
            get => _isResponseRequired;
            set
            {
                if (_isResponseRequired != value) {
                    _isResponseRequired = value;
                    OnResponseRequirementChanged?.Invoke(_isResponseRequired);
                }
            }
        }

        public string RequiredCardType = "";
        public int ActivePlayerId = -1;
        public bool IsLocalTurn => ActivePlayerId == Constants.USER_ID;

        // Observer Pattern Events
        public delegate void StateChanged(string newState);
        public event StateChanged OnStateChanged;

        public delegate void ResponseRequirementChanged(bool isRequired);
        public event ResponseRequirementChanged OnResponseRequirementChanged;

        public delegate void TimerStarted(int seconds, string message, int playerId);
        public event TimerStarted OnTimerStarted;

        public delegate void TimerCancelled();
        public event TimerCancelled OnTimerCancelled;

        public delegate void TurnStarted(int activePlayerId);
        public event TurnStarted OnTurnStarted;

        public delegate void GameSetupCompleted();
        public event GameSetupCompleted OnGameSetupCompleted;

        public void TriggerTimerStarted(int seconds, string message, int playerId) => OnTimerStarted?.Invoke(seconds, message, playerId);
        public void TriggerTimerCancelled() => OnTimerCancelled?.Invoke();
        public void TriggerTurnStarted(int activePlayerId) => OnTurnStarted?.Invoke(activePlayerId);
        public void TriggerGameSetupCompleted() => OnGameSetupCompleted?.Invoke();

        public void Clear()
        {
            Players.Clear();
            PlayerOrder.Clear();
            _isResponseRequired = false;
            RequiredCardType = "";
            _state = "";
        }

        public PlayerData GetLocalPlayer()
        {
            if (Players.ContainsKey(Constants.USER_ID))
                return Players[Constants.USER_ID];
            return null;
        }
    }
}
