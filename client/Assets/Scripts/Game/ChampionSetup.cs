using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

public class ChampionSetup : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject championPrefab; // Base champion prefab
    public GameObject skillPrefab;

    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-17.42f, 0.1f, -8.96f),
        new Vector3(17.35f, 0.1f, 0.13f),
        new Vector3(11.36f, 0.1f, 8.98f),
        new Vector3(0.5f, 0.1f, 8.83f),
        new Vector3(-10.41f, 0.1f, 8.96f),
        new Vector3(-17.49f, 0.1f, 0.21f)
    };

    private Dictionary<Transform, Coroutine> activeMoveCoroutines = new Dictionary<Transform, Coroutine>();

    private HandManager _handManager;
    private CardTargetSelector _cardTargetSelector;
    private Transform _skillContainer;

    public void Init(HandManager handManager, CardTargetSelector cardTargetSelector, ProjectH.UI.CanvasView canvasView)
    {
        _handManager = handManager;
        _cardTargetSelector = cardTargetSelector;
        if (canvasView != null) _skillContainer = canvasView.skillContainer;
    }

    public void UpdateChampionPositions(List<int> newOrder)
    {
        if (newOrder == null || newOrder.Count == 0) return;

        int totalPlayers = newOrder.Count;
        int localIdIndex = newOrder.IndexOf(Constants.USER_ID);
        if (localIdIndex == -1) localIdIndex = 0;

        for (int i = 0; i < totalPlayers; i++)
        {
            int playerId = newOrder[i];
            if (GameSession.Instance.Players.TryGetValue(playerId, out PlayerData data))
            {
                if (data.ChampionObject != null)
                {
                    int visualIndex = (i - localIdIndex + totalPlayers) % totalPlayers;
                    Vector3 targetPos = (visualIndex < spawnPositions.Length) ? spawnPositions[visualIndex] : Vector3.zero;
                    
                    Transform targetTr = data.ChampionObject.transform;
                    if (activeMoveCoroutines.TryGetValue(targetTr, out Coroutine existing))
                    {
                        if (existing != null) StopCoroutine(existing);
                    }
                    activeMoveCoroutines[targetTr] = StartCoroutine(MoveChampionRoutine(targetTr, targetPos, 0.5f));
                }
            }
        }
    }

    private System.Collections.IEnumerator MoveChampionRoutine(Transform tr, Vector3 targetPos, float duration)
    {
        Vector3 startPos = tr.position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            if (tr == null) break;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t); // SmoothStep
            tr.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        if (tr != null)
        {
            tr.position = targetPos;
            activeMoveCoroutines.Remove(tr);
        }
    }

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
                    SpecialRange = info.Champion.SpecialRange,
                    SpecialDefense = info.Champion.SpecialDefense,
                    AdditionalTargetForAttack = info.Champion.AdditionalTargetForAttack,
                    MaxNumOfAttack = info.Champion.MaxNumOfAttack,
                    CurNumOfAttack = info.Champion.CurNumOfAttack,
                    SkillIds = info.Champion.SkillIds
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

                // Instantiate Skills ONLY for the local player
                if (info.PlayerId == Constants.USER_ID && skillPrefab != null && _skillContainer != null && data.Champion.SkillIds != null)
                {
                    foreach (int skillId in data.Champion.SkillIds)
                    {
                        SkillSO skillSO = Resources.Load<SkillSO>($"Data/Skills/{skillId}");
                        if (skillSO != null)
                        {
                            GameObject skillGo = Instantiate(skillPrefab, _skillContainer);
                            skillGo.name = $"Skill_{skillSO.skillName}";
                            
                            SkillUIController skillUI = skillGo.GetComponent<SkillUIController>();
                            if (skillUI != null)
                            {
                                skillUI.Init(skillSO, _handManager, _cardTargetSelector);
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[ChampionSetup] SkillSO with ID {skillId} not found in Resources/Data/Skills/");
                        }
                    }
                }
            }

            GameSession.Instance.Players[info.PlayerId] = data;
            GameSession.Instance.PlayerOrder.Add(info.PlayerId);
        }
    }
}
