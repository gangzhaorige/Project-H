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
        public int SpecialRange;
        public int CurNumOfAttack;
        public int MaxNumOfAttack;
        public int AdditionalTargetForAttack;
        public int SpecialDefense;
        public List<int> SkillIds = new List<int>();
        public List<List<AudioClip>> SkillClips = new List<List<AudioClip>>();
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

        public bool IsAlive => Champion != null && Champion.CurHP > 0;

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

        public int RequiredCardType = 0;
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

        public delegate void ChampionStatsUpdated(int championId, int statId, int value);
        public event ChampionStatsUpdated OnChampionStatsUpdated;

        public delegate void SkillQuery(int skillId);
        public event SkillQuery OnSkillQuery;

        public delegate void SkillQueryAnswered();
        public event SkillQueryAnswered OnSkillQueryAnswered;

        public delegate void SkillActivated(int playerId, int skillIndex);
        public event SkillActivated OnSkillActivated;

        public delegate void SelectCardsFromOpponent(int targetPlayerId, int amount, int duration, string message, int targetHandSize);
        public event SelectCardsFromOpponent OnSelectCardsFromOpponent;

        public delegate void SelectCardsCompleted();
        public event SelectCardsCompleted OnSelectCardsCompleted;

        public void TriggerTimerStarted(int seconds, string message, int playerId) => OnTimerStarted?.Invoke(seconds, message, playerId);
        public void TriggerTimerCancelled() => OnTimerCancelled?.Invoke();
        public void TriggerTurnStarted(int activePlayerId) => OnTurnStarted?.Invoke(activePlayerId);
        public void TriggerGameSetupCompleted() => OnGameSetupCompleted?.Invoke();
        public void TriggerChampionStatsUpdated(int championId, int statId, int value) => OnChampionStatsUpdated?.Invoke(championId, statId, value);
        public void TriggerSkillQuery(int skillId) => OnSkillQuery?.Invoke(skillId);
        public void TriggerSkillQueryAnswered() => OnSkillQueryAnswered?.Invoke();
        public void TriggerSkillActivated(int playerId, int skillIndex) => OnSkillActivated?.Invoke(playerId, skillIndex);
        public void TriggerSelectCardsFromOpponent(int targetId, int amount, int duration, string msg, int handSize) => OnSelectCardsFromOpponent?.Invoke(targetId, amount, duration, msg, handSize);
        public void TriggerSelectCardsCompleted() => OnSelectCardsCompleted?.Invoke();

        // Stat IDs (Matching Server)
        public const int STAT_CUR_HP = 1;
        public const int STAT_MAX_HP = 2;
        public const int STAT_ATTACK = 3;
        public const int STAT_ATTACK_RANGE = 4;
        public const int STAT_SPECIAL_RANGE = 5;
        public const int STAT_CUR_NUM_ATTACK = 6;
        public const int STAT_MAX_NUM_ATTACK = 7;
        public const int STAT_SPECIAL_DEFENSE_RANGE = 8;
        public const int STAT_ADDITIONAL_TARGET_FOR_ATTACK = 9;
        public const int STAT_PATH_ID = 10;
        public const int STAT_ELEMENT = 11;

        public void Clear()
        {
            Players.Clear();
            PlayerOrder.Clear();
            _isResponseRequired = false;
            RequiredCardType = 0;
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
