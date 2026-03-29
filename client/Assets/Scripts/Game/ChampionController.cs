using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectH.Models;
using System.Collections.Generic;

public class ChampionController : MonoBehaviour
{
    [Header("Stats")]
    public int id;
    public string championName;
    public int maxHP;
    public int curHP;
    public int pathId;
    public int element;
    public int attack;
    public int attackRange;
    public int specialDefense;

    [Header("UI References")]
    [SerializeField] private Image championImageUI;
    [SerializeField] private TextMeshProUGUI hpTextUI;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    [SerializeField] private Image elementImageUI;
    [SerializeField] private Image pathImageUI;
    [SerializeField] private TextMeshProUGUI attackTextUI;
    [SerializeField] private TextMeshProUGUI rangeTextUI;
    [SerializeField] private TextMeshProUGUI cardCountTextUI;
    [SerializeField] private GameObject activeIndicator;

    [Header("Debug Data")]
    public ChampionData championData;

    private PlayerData player;
    private ChampionSO cachedSO;

    public void Init(PlayerData pData)
    {
        this.player = pData;
        this.championData = pData.Champion;
        ChampionData data = pData.Champion;

        this.id = data.Id;
        this.championName = data.Name;
        this.maxHP = data.MaxHP;
        this.curHP = data.CurHP;
        this.pathId = data.PathId;
        this.element = data.Element;
        this.attack = data.Attack;
        this.attackRange = data.AttackRange;
        this.specialDefense = data.SpecialDefense;

        // Subscribe to hand changes
        player.OnHandChanged += UpdateCardCount;
        
        // Subscribe to stat updates (Observer Pattern)
        GameSession.Instance.OnChampionStatsUpdated += OnChampionStatsUpdated;

        LoadChampionAssets();
        UpdateCardCount();
    }

    private void LoadChampionAssets()
    {
        ChampionAssetManager.Instance.GetChampionSO(id, (so) =>
        {
            cachedSO = so;
            UpdateVisuals();
        });
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHandChanged -= UpdateCardCount;
        }
        
        if (GameSession.Instance != null)
        {
            GameSession.Instance.OnChampionStatsUpdated -= OnChampionStatsUpdated;
        }
    }

    private void OnChampionStatsUpdated(int championId, int statId, int value)
    {
        if (this.id != championId) return;

        switch (statId)
        {
            case GameSession.STAT_CUR_HP: this.curHP = value; break;
            case GameSession.STAT_MAX_HP: this.maxHP = value; break;
            case GameSession.STAT_ATTACK: this.attack = value; break;
            case GameSession.STAT_ATTACK_RANGE: this.attackRange = value; break;
        }

        UpdateVisuals();
    }

    private void UpdateCardCount()
    {
        if (cardCountTextUI != null)
        {
            cardCountTextUI.text = player.Hand.Count.ToString();
        }
    }

    public void ToggleActive(bool active)
    {
        if (activeIndicator != null)
        {
            activeIndicator.SetActive(active);
        }
    }

    private void UpdateVisuals()
    {
        if (nameTextUI != null) nameTextUI.text = championName;
        if (hpTextUI != null) hpTextUI.text = $"{curHP}";
        if (attackTextUI != null) attackTextUI.text = attack.ToString();
        if (rangeTextUI != null) rangeTextUI.text = attackRange.ToString();

        if (cachedSO == null) return;

        // Use ChampionAssetManager to share sprites across multiple components
        ChampionAssetManager.Instance.GetSprite(cachedSO.champInGameImage, (s) => { if (championImageUI != null) championImageUI.sprite = s; });
        ChampionAssetManager.Instance.GetSprite(cachedSO.elementImage, (s) => { if (elementImageUI != null) elementImageUI.sprite = s; });
        ChampionAssetManager.Instance.GetSprite(cachedSO.pathImage, (s) => { if (pathImageUI != null) pathImageUI.sprite = s; });
    }
}
