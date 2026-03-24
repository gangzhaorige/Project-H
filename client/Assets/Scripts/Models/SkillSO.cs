using UnityEngine;
using System.Collections.Generic;

namespace ProjectH.Models
{
    public enum PlayCondition
    {
        IsPlayerTurn,
        IsPlayerActionState,
        OncePerTurn,
        HandAtleastTwoCards
    }

    [System.Serializable]
    public class CardRequirement
    {
        [Tooltip("Number of cards required to be discarded to activate this skill")]
        public int discardCount;
    }

    [CreateAssetMenu(fileName = "New Skill", menuName = "ProjectH/Skill")]
    public class SkillSO : ScriptableObject
    {
        public int skillId;
        public string skillName;
        [TextArea(3, 5)]
        public string skillDescription;
        public bool isPassive;
        public Sprite skillIcon;
        
        [Header("Conditions")]
        public List<PlayCondition> playConditions = new List<PlayCondition>();
        
        [Header("Targeting")]
        public int numberOfTargetToSelect;
        
        [Header("Costs / Requirements")]
        public CardRequirement cardRequirement;
    }
}
