using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Icy Cage")]
    public class IcyCageSkillConfig : SkillConfig<IcyCageLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private IcyCage projectilePrefab;
        [SerializeField] private FreezeBuffDefinition freezeBuffDefinition;

        public IcyCage ProjectilePrefab => projectilePrefab;
        public FreezeBuffDefinition FreezeBuffDefinition => freezeBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(freezeBuffDefinition, nameof(freezeBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class IcyCageLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        [Header("消耗")]
        [Tooltip("释放所需能量")]
        public float RequiredEnergy = 65f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
