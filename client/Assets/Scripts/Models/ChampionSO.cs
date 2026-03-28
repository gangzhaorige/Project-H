using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace ProjectH.Models
{
    public enum PathType { Destruction = 0, Hunt = 1, Erudition = 2, Harmony = 3, Nihility = 4, Preservation = 5, Abundance = 6 }
    public enum ElementType { None = 0, Physical = 1, Fire = 2, Ice = 3, Lightning = 4, Wind = 5, Quantum = 6, Imaginary = 7 }

    [System.Serializable]
    public class SkillAudio
    {
        public List<AudioClip> clips = new List<AudioClip>();
    }

    [CreateAssetMenu(fileName = "New Champion", menuName = "ProjectH/Champion")]
    public class ChampionSO : ScriptableObject
    {
        public int id;
        public string championName;
        public PathType path;
        public ElementType element;

        [Header("Visuals")]
        public AssetReferenceSprite champIcon;
        public AssetReferenceSprite champSelectImage;
        public AssetReferenceSprite champInGameImage;
        public AssetReferenceSprite pathImage;
        public AssetReferenceSprite elementImage;

        [Header("Audio")]
        public List<SkillAudio> skillAudio = new List<SkillAudio>();
        public AudioClip damageTaken;
    }
}
