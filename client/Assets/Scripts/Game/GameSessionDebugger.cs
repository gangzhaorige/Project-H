using UnityEngine;
using ProjectH.Models;
using System.Collections.Generic;

namespace ProjectH.Debug
{
    /**
     * Component to view GameSession data in the Unity Inspector.
     * Attach this to a persistent object in the Game scene.
     */
    public class GameSessionDebugger : MonoBehaviour
    {
        [Header("Runtime Data (View Only)")]
        public string currentState;
        public int activePlayerId;
        public List<int> playerOrder;
        
        [Space]
        public List<PlayerData> players = new List<PlayerData>();

        private void Update()
        {
            if (GameSession.Instance == null) return;

            currentState = GameSession.Instance.State;
            activePlayerId = GameSession.Instance.ActivePlayerId;
            playerOrder = GameSession.Instance.PlayerOrder;

            // Sync players list for visualization
            players.Clear();
            foreach (var p in GameSession.Instance.Players.Values)
            {
                players.Add(p);
            }
        }
    }
}
