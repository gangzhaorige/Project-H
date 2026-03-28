using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectH.Models;

public class GameplayManager : MonoBehaviour
{
    private HandManager _handManager;
    private ChampionSetup _championSetup;

    public void Init(HandManager handManager, CardTargetSelector cardTargetSelector, ProjectH.UI.CanvasView canvasView, ChampionSetup championSetup)
    {
        _handManager = handManager;
        _championSetup = championSetup;
        
        if (_championSetup != null)
        {
            _championSetup.Init(handManager, cardTargetSelector, canvasView);
        }
        else
        {
            Debug.LogError("[GameplayManager] CRITICAL: championSetup reference is null in Init!");
        }

        GameSession.Instance.Clear();

        Debug.Log("[GameplayManager] Registering callbacks.");
        NetworkManager.Instance.AddCallback(Constants.SMSG_GAME_SETUP, OnGameSetup);
        NetworkManager.Instance.AddCallback(Constants.SMSG_CARD_DRAW, OnCardDraw);
        NetworkManager.Instance.AddCallback(Constants.SMSG_CARD_DRAW_OTHER, OnCardDrawOther);
        NetworkManager.Instance.AddCallback(Constants.SMSG_TURN_START, OnTurnStart);
        NetworkManager.Instance.AddCallback(Constants.SMSG_END_TURN, OnTurnEnd);
        NetworkManager.Instance.AddCallback(Constants.SMSG_RESPONSE_TIMER_START, OnTimerStart);
        NetworkManager.Instance.AddCallback(Constants.SMSG_RESPONSE_TIMER_CANCEL, OnTimerCancel);
        NetworkManager.Instance.AddCallback(Constants.SMSG_PASS_PRIORITY, OnPassPriorityResponse);
        NetworkManager.Instance.AddCallback(Constants.SMSG_STATE_CHANGE, OnGameStateResponse);
        NetworkManager.Instance.AddCallback(Constants.SMSG_CHAMPION_STATS_UPDATE_INTEGER, OnChampionStatsUpdate);
        NetworkManager.Instance.AddCallback(Constants.SMSG_SKILL_ACTIVATION, OnSkillActivated);
        NetworkManager.Instance.AddCallback(Constants.SMSG_SELECT_CARDS_FROM_OPPONENT, OnSelectCardsFromOpponent);
        NetworkManager.Instance.AddCallback(Constants.SMSG_SELECT_CARDS, OnSelectCardsResponse);
        NetworkManager.Instance.AddCallback(Constants.SMSG_MOVE_CARD, OnMoveCardResponse);
        NetworkManager.Instance.AddCallback(Constants.SMSG_UPDATE_PLAYER_ORDER, OnUpdatePlayerOrder);

        // Handshake Step 1: Tell server we loaded the scene and are ready for data
        Debug.Log("[GameplayManager] Sending RequestReadyForGameSetup...");
        RequestReadyForGameSetup setupReq = new RequestReadyForGameSetup();
        setupReq.Send();
        NetworkManager.Instance.SendRequest(setupReq);
    }

    void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_GAME_SETUP);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_CARD_DRAW);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_CARD_DRAW_OTHER);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_TURN_START);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_END_TURN);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_RESPONSE_TIMER_START);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_RESPONSE_TIMER_CANCEL);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_PASS_PRIORITY);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_STATE_CHANGE);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_CHAMPION_STATS_UPDATE_INTEGER);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_SKILL_ACTIVATION);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_SELECT_CARDS_FROM_OPPONENT);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_SELECT_CARDS);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_MOVE_CARD);
            NetworkManager.Instance.RemoveCallback(Constants.SMSG_UPDATE_PLAYER_ORDER);
        }
    }

    private void OnGameSetup(ExtendedEventArgs args)
    {
        ResponseGameSetupEventArgs res = args as ResponseGameSetupEventArgs;
        if (res == null) return;

        StartCoroutine(EstablishSceneRoutine(res));
    }

    private IEnumerator EstablishSceneRoutine(ResponseGameSetupEventArgs res)
    {
        Debug.Log("[GameplayManager] Starting Scene Establishment...");

        // 1. Initialize Champions (Prefab Spawning)
        if (_championSetup != null) _championSetup.InitializeChampions(res.Players);
        yield return null; // Wait a frame

        // 2. Preload Addressables via ChampionAssetManager
        int totalChamps = res.Players.Count;
        int loadedChamps = 0;

        foreach (var playerInfo in res.Players)
        {
            if (playerInfo.Champion == null) { loadedChamps++; continue; }

            bool soLoaded = false;
            ChampionAssetManager.Instance.GetChampionSO(playerInfo.Champion.ChampionId, (so) =>
            {
                // We don't necessarily need to wait for individual sprites here if the components 
                // load them lazily, but preloading the SO is essential.
                // To be thorough, let's preload the key images too.
                int imagesToLoad = 3;
                int imagesLoaded = 0;

                System.Action checkImage = () => {
                    imagesLoaded++;
                    if (imagesLoaded >= imagesToLoad) {
                        loadedChamps++;
                        soLoaded = true;
                    }
                };

                ChampionAssetManager.Instance.GetSprite(so.champInGameImage, (s) => checkImage());
                ChampionAssetManager.Instance.GetSprite(so.elementImage, (s) => checkImage());
                ChampionAssetManager.Instance.GetSprite(so.pathImage, (s) => checkImage());
            });

            // Wait for this champion's critical assets
            while (!soLoaded) yield return null;
        }

        // 3. Load Audio
        LoadAllSkillAudios();
        yield return null;

        // 4. Finalize
        GameSession.Instance.TriggerGameSetupCompleted();

        Debug.Log("[GameplayManager] Scene Establishment Complete. Sending RequestReadyToPlay.");
        RequestReadyToPlay readyReq = new RequestReadyToPlay();
        readyReq.Send();
        NetworkManager.Instance.SendRequest(readyReq);
    }

    private void OnTimerStart(ExtendedEventArgs args)
    {
        ResponseTimerStartEventArgs res = args as ResponseTimerStartEventArgs;
        if (res == null) return;

        bool isNegationWindow = (res.PlayerId == -1);
        GameSession.Instance.RequiredCardType = res.RequiredCardType;
        GameSession.Instance.IsResponseRequired = (res.PlayerId == Constants.USER_ID || isNegationWindow);

        Debug.Log($"[GameplayManager] Timer start for player {res.PlayerId}. seconds: {res.Seconds}, RequiredType: {res.RequiredCardType}");

        GameSession.Instance.TriggerTimerStarted(res.Seconds, res.Message, res.PlayerId);

        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.ChampionObject != null)
            {
                var controller = player.ChampionObject.GetComponent<ChampionController>();
                if (controller != null)
                {
                    controller.ToggleActive(isNegationWindow || player.PlayerId == res.PlayerId);
                }
            }
        }
    }

    private void OnPassPriorityResponse(ExtendedEventArgs args)
    {
        ResponsePassPriorityEventArgs res = args as ResponsePassPriorityEventArgs;
        if (res == null) return;

        GameSession.Instance.IsResponseRequired = false;
        GameSession.Instance.RequiredCardType = 0;
        GameSession.Instance.TriggerTimerCancelled();
    }

    private void OnTimerCancel(ExtendedEventArgs args)
    {
        GameSession.Instance.IsResponseRequired = false;
        GameSession.Instance.RequiredCardType = 0;
        GameSession.Instance.TriggerTimerCancelled();
        
        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.ChampionObject != null)
            {
                var controller = player.ChampionObject.GetComponent<ChampionController>();
                if (controller != null) controller.ToggleActive(false);
            }
        }
    }

    private void OnCardDraw(ExtendedEventArgs args)
    {
        ResponseDrawCardEventArgs res = args as ResponseDrawCardEventArgs;
        if (res != null && _handManager != null) _handManager.HandleLocalDraw(res.Cards);
    }

    private void OnCardDrawOther(ExtendedEventArgs args)
    {
        ResponseDrawCardOtherEventArgs res = args as ResponseDrawCardOtherEventArgs;
        if (res != null && _handManager != null) _handManager.HandleOtherDraw(res.PlayerId, res.CardCount);
    }

    private void OnTurnStart(ExtendedEventArgs args)
    {
        ResponseTurnStartEventArgs res = args as ResponseTurnStartEventArgs;
        if (res == null) return;

        Debug.Log($"[GameplayManager] Turn started for player {res.ActivePlayerId}.");
        GameSession.Instance.ActivePlayerId = res.ActivePlayerId;
        GameSession.Instance.TriggerTurnStarted(res.ActivePlayerId);
    }

    private void OnTurnEnd(ExtendedEventArgs args)
    {
        ResponseEndTurnEventArgs res = args as ResponseEndTurnEventArgs;
        if (res != null) Debug.Log($"[GameplayManager] Turn ended for player {res.EndedPlayerId}");
        
        GameSession.Instance.ActivePlayerId = -1;
        GameSession.Instance.TriggerTimerCancelled();

        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.ChampionObject != null)
            {
                var controller = player.ChampionObject.GetComponent<ChampionController>();
                if (controller != null) controller.ToggleActive(false);
            }
        }
    }

    private void OnSelectCardsFromOpponent(ExtendedEventArgs args)
    {
        ResponseSelectCardsFromOpponentEventArgs res = args as ResponseSelectCardsFromOpponentEventArgs;
        if (res == null) return;

        Debug.Log($"[GameplayManager] Opponent selection requested: {res.Amount} card(s) from {res.TargetPlayerId}");
        GameSession.Instance.TriggerSelectCardsFromOpponent(res.TargetPlayerId, res.Amount, res.Duration, res.Message, res.TargetHandSize);
    }

    private void OnSelectCardsResponse(ExtendedEventArgs args)
    {
        Debug.Log("[GameplayManager] Card selection completed/confirmed by server.");
        GameSession.Instance.TriggerSelectCardsCompleted();
    }

    private void OnMoveCardResponse(ExtendedEventArgs args)
    {
        ResponseMoveCardEventArgs res = args as ResponseMoveCardEventArgs;
        if (res == null) return;

        Debug.Log($"[GameplayManager] Move Card event: {res.Cards.Count} cards from {res.TargetId} to {res.CasterId}. Details Visible: {res.ShowDetails}");
        if (_handManager != null) _handManager.HandleMoveCard(res);
    }

    private void OnUpdatePlayerOrder(ExtendedEventArgs args)
    {
        ResponseUpdatePlayerOrderEventArgs res = args as ResponseUpdatePlayerOrderEventArgs;
        if (res == null) return;

        Debug.Log($"[GameplayManager] Updating Player Order: {string.Join(", ", res.PlayerOrder)}");
        GameSession.Instance.PlayerOrder = res.PlayerOrder;
        
        if (_championSetup != null)
        {
            _championSetup.UpdateChampionPositions(res.PlayerOrder);
        }
    }

    private void OnSkillActivated(ExtendedEventArgs args)
    {
        ResponseSkillActivatedEventArgs res = args as ResponseSkillActivatedEventArgs;
        if (res == null) return;

        Debug.Log($"[GameplayManager] Skill {res.SkillIndex} activated for Player {res.PlayerId}");
        GameSession.Instance.TriggerSkillActivated(res.PlayerId, res.SkillIndex);

        if (GameSession.Instance.Players.TryGetValue(res.PlayerId, out PlayerData player))
        {
            if (player.Champion != null && player.Champion.SkillClips != null && res.SkillIndex < player.Champion.SkillClips.Count)
            {
                List<AudioClip> clips = player.Champion.SkillClips[res.SkillIndex];
                if (clips != null && clips.Count > 0)
                {
                    AudioClip clip = clips[Random.Range(0, clips.Count)];
                    if (clip != null && AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySkillAudio(clip);
                    }
                }
            }
        }
    }

    private void LoadAllSkillAudios()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/champions");
        if (jsonText == null) return;

        string rawJson = jsonText.text;

        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.Champion == null) continue;
            player.Champion.SkillClips = new List<List<AudioClip>>();
            
            string pattern = $"\"id\":\\s*{player.Champion.Id},.*?\"skillAudio\":\\s*\\[(.*?)\\]\\s*[\\}},]";
            var match = System.Text.RegularExpressions.Regex.Match(rawJson, pattern, System.Text.RegularExpressions.RegexOptions.Singleline);
            
            if (match.Success)
            {
                string skillAudioSection = match.Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(skillAudioSection)) continue;

                string[] groups = skillAudioSection.Split(new string[] { "]," }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string group in groups)
                {
                    List<AudioClip> clipsInGroup = new List<AudioClip>();
                    string cleaned = group.Replace("[", "").Replace("]", "");
                    string[] files = cleaned.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (string f in files)
                    {
                        string fileName = f.Trim().Trim('"');
                        if (string.IsNullOrEmpty(fileName)) continue;
                        AudioClip clip = Resources.Load<AudioClip>($"Audio/ChampionVoices/{fileName}");
                        if (clip != null) clipsInGroup.Add(clip);
                    }
                    player.Champion.SkillClips.Add(clipsInGroup);
                }
            }
        }
    }

    private void OnGameStateResponse(ExtendedEventArgs args) {
        ResponseGameStateEventArgs res = args as ResponseGameStateEventArgs;
        if (res != null && res.Status == Constants.SUCCESS) {
            Debug.Log("[GameplayManager] State Change: " + res.StateName);
            GameSession.Instance.State = res.StateName;
        }
    }

    private void OnChampionStatsUpdate(ExtendedEventArgs args)
    {
        ResponseChampionStatsUpdateIntegerEventArgs res = args as ResponseChampionStatsUpdateIntegerEventArgs;
        if (res == null) return;

        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.Champion != null && player.Champion.Id == res.ChampionId)
            {
                switch (res.StatId)
                {
                    case GameSession.STAT_CUR_HP: player.Champion.CurHP = res.Value; break;
                    case GameSession.STAT_MAX_HP: player.Champion.MaxHP = res.Value; break;
                    case GameSession.STAT_ATTACK: player.Champion.Attack = res.Value; break;
                    case GameSession.STAT_ATTACK_RANGE: player.Champion.AttackRange = res.Value; break;
                    case GameSession.STAT_CUR_NUM_ATTACK: player.Champion.CurNumOfAttack = res.Value; break;
                    case GameSession.STAT_MAX_NUM_ATTACK: player.Champion.MaxNumOfAttack = res.Value; break;
                    case GameSession.STAT_ADDITIONAL_TARGET_FOR_ATTACK: player.Champion.AdditionalTargetForAttack = res.Value; break;
                }
                GameSession.Instance.TriggerChampionStatsUpdated(res.ChampionId, res.StatId, res.Value);
                break;
            }
        }
    }
}
