using UnityEngine;
using System.Collections.Generic;
using ProjectH.Models;

/**
 * Manages the UI animation for skill activation.
 */
public class SkillActivationManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject skillActivationPrefab;
    
    [Header("UI Containers")]
    public Transform uiContainer; // The canvas or parent where animation should live

    private Dictionary<int, Sprite> championIcons = new Dictionary<int, Sprite>();

    private void OnEnable()
    {
        GameSession.Instance.OnGameSetupCompleted += OnGameSetup;
        GameSession.Instance.OnSkillActivated += OnSkillActivated;
    }

    private void OnDisable()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.OnGameSetupCompleted -= OnGameSetup;
            GameSession.Instance.OnSkillActivated -= OnSkillActivated;
        }
    }

    private void OnGameSetup()
    {
        // Create championId -> Sprite mapper
        championIcons.Clear();
        foreach (var player in GameSession.Instance.Players.Values)
        {
            if (player.Champion != null)
            {
                int champId = player.Champion.Id;
                if (!championIcons.ContainsKey(champId))
                {
                    Sprite icon = Resources.Load<Sprite>($"Images/Characters/Icons/{champId}");
                    if (icon == null) icon = Resources.Load<Sprite>("Images/Characters/Icons/default");
                    championIcons[champId] = icon;
                }
            }
        }
    }

    private void OnSkillActivated(int playerId, int skillIndex)
    {
        if (skillActivationPrefab == null || uiContainer == null) return;

        if (GameSession.Instance.Players.TryGetValue(playerId, out PlayerData player))
        {
            if (player.Champion != null)
            {
                // Instantiate the prefab
                GameObject go = Instantiate(skillActivationPrefab, uiContainer);
                
                // Get components
                ChampionSkillPanel skillPanel = go.GetComponent<ChampionSkillPanel>();
                DualUISlideIn slideIn = go.GetComponent<DualUISlideIn>();

                if (skillPanel != null)
                {
                    // 0. Set Theme based on element
                    skillPanel.SetElementTheme(player.Champion.Element);

                    // 1. Get Sprite
                    if (championIcons.TryGetValue(player.Champion.Id, out Sprite champIcon))
                    {
                        skillPanel.UpdatePanelInfo(champIcon);
                    }

                    // 2. Get Skill Name
                    // We need to find the skill ID based on index
                    if (skillIndex < player.Champion.SkillIds.Count)
                    {
                        int skillId = player.Champion.SkillIds[skillIndex];
                        SkillSO skillSO = Resources.Load<SkillSO>($"Data/Skills/{skillId}");
                        if (skillSO != null)
                        {
                            skillPanel.UpdateText(skillSO.skillName);
                        }
                    }
                }

                if (slideIn != null)
                {
                    // 3. Animate it
                    slideIn.PlayAnimationAndFadeOut(() => {
                        // 4. On animation complete destroy the object
                        Destroy(go);
                    });
                }
                else
                {
                    // Fallback if script missing
                    Destroy(go, 2f);
                }
            }
        }
    }
}
