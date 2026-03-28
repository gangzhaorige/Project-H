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
    
    private Transform _uiContainer;
    private Dictionary<int, Sprite> championIcons = new Dictionary<int, Sprite>();

    public void Init(ProjectH.UI.CanvasView canvasView)
    {
        if (canvasView != null)
        {
            _uiContainer = canvasView.skillActivationPanel;
        }
    }

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
        // We will load sprites on demand now using ChampionAssetManager
        championIcons.Clear();
    }

    private void OnSkillActivated(int playerId, int skillIndex)
    {
        if (skillActivationPrefab == null || _uiContainer == null) return;

        if (GameSession.Instance.Players.TryGetValue(playerId, out PlayerData player))
        {
            if (player.Champion != null)
            {
                // Instantiate the prefab
                GameObject go = Instantiate(skillActivationPrefab, _uiContainer);
                
                // Get components
                ChampionSkillPanel skillPanel = go.GetComponent<ChampionSkillPanel>();
                DualUISlideIn slideIn = go.GetComponent<DualUISlideIn>();

                if (skillPanel != null)
                {
                    // 0. Set Theme based on element
                    skillPanel.SetElementTheme(player.Champion.Element);

                    // 1. Get Sprite asynchronously
                    ChampionAssetManager.Instance.GetChampionSO(player.Champion.Id, (so) => 
                    {
                        ChampionAssetManager.Instance.GetSprite(so.champIcon, (s) => 
                        {
                            if (skillPanel != null && s != null) skillPanel.UpdatePanelInfo(s);
                        });
                    });

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
