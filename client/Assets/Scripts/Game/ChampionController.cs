using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectH.Models;

public class ChampionController : MonoBehaviour
{
    [Header("Stats")]
    public int id;
    public string championName;
    public int maxHP;
    public int curHP;
    public int pathId;
    public string element;
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

    private PlayerData player;

    public void Init(PlayerData pData)
    {
        this.player = pData;
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

        UpdateVisuals();
        UpdateCardCount();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHandChanged -= UpdateCardCount;
        }
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
            Debug.Log($"[ChampionController] {championName} active indicator -> {active}");
            activeIndicator.SetActive(active);
        }
    }

    private void UpdateVisuals()
    {
        if (nameTextUI != null) nameTextUI.text = championName;
        if (hpTextUI != null) hpTextUI.text = $"{curHP}";
        if (attackTextUI != null) attackTextUI.text = attack.ToString();
        if (rangeTextUI != null) rangeTextUI.text = attackRange.ToString();

        if (championImageUI != null)
        {
            championImageUI.sprite = Resources.Load<Sprite>($"Images/Characters/Portraits/InGame/{id}");
        }

        if (elementImageUI != null)
        {
            elementImageUI.sprite = Resources.Load<Sprite>($"Images/Elements/{element}");
        }

        if (pathImageUI != null)
        {
            pathImageUI.sprite = Resources.Load<Sprite>($"Images/Paths/{pathId}");
        }

        Debug.Log($"Initialized {championName} UI with {curHP}/{maxHP} HP");
    }
}
