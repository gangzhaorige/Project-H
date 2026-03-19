using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class ChampionSetup : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject championPrefab; // Base champion prefab

    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-17.42f, 0.1f, -8.96f),
        new Vector3(17.35f, 0.1f, 0.13f),
        new Vector3(11.36f, 0.1f, 8.98f),
        new Vector3(0.5f, 0.1f, 8.83f),
        new Vector3(-10.41f, 0.1f, 8.96f),
        new Vector3(-17.49f, 0.1f, 0.21f)
    };

    public void InitializeChampions(List<PlayerSetupInfo> players)
    {
        Debug.Log($"InitializeChampions called with {players.Count} players.");
        if (championPrefab == null) {
            Debug.LogError("championPrefab is NOT assigned in ChampionSetup!");
            return;
        }

        // 1. Find the local player's index in the server-provided list
        int localPlayerServerIndex = -1;
        foreach (var p in players)
        {
            if (p.PlayerId == Constants.USER_ID)
            {
                localPlayerServerIndex = p.PlayerIndex;
                break;
            }
        }

        if (localPlayerServerIndex == -1)
        {
            Debug.LogWarning("Local player not found in setup list. Defaulting to index 0.");
            localPlayerServerIndex = 0;
        }

        int totalPlayers = players.Count;

        for (int i = 0; i < players.Count; i++)
        {
            var info = players[i];
            
            // 2. Calculate the "visual index" relative to the local player
            // Formula: (targetIndex - localIndex + totalPlayers) % totalPlayers
            // This ensures local player is always index 0, and others follow clockwise
            int visualIndex = (info.PlayerIndex - localPlayerServerIndex + totalPlayers) % totalPlayers;

            Debug.Log($"Processing player {info.Username} (ServerIdx: {info.PlayerIndex}, VisualIdx: {visualIndex})");
            
            // Store data in Session
            PlayerData data = new PlayerData
            {
                PlayerId = info.PlayerId,
                Username = info.Username,
                Team = info.Team
            };

            if (info.Champion != null)
            {
                Debug.Log($"Spawning champion {info.Champion.ChampionName} for {info.Username} at VisualIdx {visualIndex}");
                data.Champion = new ChampionData
                {
                    Id = info.Champion.ChampionId,
                    Name = info.Champion.ChampionName,
                    MaxHP = info.Champion.MaxHP,
                    CurHP = info.Champion.CurHP,
                    PathId = info.Champion.PathId,
                    Element = info.Champion.Element,
                    Attack = info.Champion.Attack,
                    AttackRange = info.Champion.AttackRange,
                    SpecialDefense = info.Champion.SpecialDefense,
                    AdditionalTargetForAttack = info.Champion.AdditionalTargetForAttack,
                    MaxNumOfAttack = info.Champion.MaxNumOfAttack,
                    CurNumOfAttack = info.Champion.CurNumOfAttack
                };

                // Spawn at the position corresponding to the visual index
                Vector3 position = (visualIndex < spawnPositions.Length) ? spawnPositions[visualIndex] : Vector3.zero;
                Debug.Log($"Instantiating at {position}");

                GameObject go = Instantiate(championPrefab, position, championPrefab.transform.rotation);
                go.name = $"Champion_{info.Username}";
                data.ChampionObject = go;

                ChampionController controller = go.GetComponent<ChampionController>();
                if (controller == null) controller = go.AddComponent<ChampionController>();
                controller.Init(data);
            }

            GameSession.Instance.Players[info.PlayerId] = data;
            GameSession.Instance.PlayerOrder.Add(info.PlayerId);
        }
    }
}
