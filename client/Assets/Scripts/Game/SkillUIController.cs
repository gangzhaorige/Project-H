using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectH.Models;

public class SkillUIController : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public Button skillButton; // Make sure to assign this in the inspector

    private SkillSO skillData;

    public void Init(SkillSO skillSO)
    {
        skillData = skillSO;

        if (nameText != null)
        {
            nameText.text = skillData.skillName;
        }

        if (iconImage != null && skillData.skillIcon != null)
        {
            iconImage.sprite = skillData.skillIcon;
        }

        if (skillButton != null)
        {
            skillButton.interactable = false;
        }

        GameSession.Instance.OnSkillQuery += OnSkillQueryReceived;
        GameSession.Instance.OnSkillQueryAnswered += OnSkillQueryAnswered;
        GameSession.Instance.OnTimerCancelled += OnTimerCancelled;
        
        Debug.Log($"Initialized Skill UI for: {skillData.skillName}");
    }

    private void OnDestroy()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.OnSkillQuery -= OnSkillQueryReceived;
            GameSession.Instance.OnSkillQueryAnswered -= OnSkillQueryAnswered;
            GameSession.Instance.OnTimerCancelled -= OnTimerCancelled;
        }
    }

    private void OnSkillQueryReceived(int querySkillId)
    {
        if (skillData != null && querySkillId == skillData.skillId)
        {
            if (skillButton != null) 
            {
                skillButton.interactable = true;
            }
        }
    }

    private void OnSkillQueryAnswered()
    {
        if (skillButton != null) 
        {
            skillButton.interactable = false;
        }
    }

    private void OnTimerCancelled()
    {
        if (skillButton != null) 
        {
            skillButton.interactable = false;
        }
    }
}
