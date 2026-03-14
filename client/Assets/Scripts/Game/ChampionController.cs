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

    [Header("UI References")]
    [SerializeField] private Image championImageUI;
    [SerializeField] private TextMeshProUGUI hpTextUI;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    [SerializeField] private Image elementImageUI;
    [SerializeField] private Image pathImageUI;
    [SerializeField] private TextMeshProUGUI attackTextUI;
    [SerializeField] private TextMeshProUGUI rangeTextUI;

    public void Init(ChampionData data)
    {
        this.id = data.Id;
        this.championName = data.Name;
        this.maxHP = data.MaxHP;
        this.curHP = data.CurHP;
        this.pathId = data.PathId;
        this.element = data.Element;
        this.attack = data.Attack;
        this.attackRange = data.AttackRange;

        UpdateVisuals();
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
