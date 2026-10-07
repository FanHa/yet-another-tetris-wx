using UnityEngine;
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
        [SerializeField] private BuffDefinition freezeBuffDefinition;

        public IcyCage ProjectilePrefab => projectilePrefab;
        public BuffDefinition FreezeBuffDefinition => freezeBuffDefinition;

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
        [Header("冻结效果")]
        [Tooltip("基础冻结持续时间")]
        public float BaseFreezeDuration = 2f;
        [Tooltip("每个冰块增加的冻结持续时间")]
        public float FreezeDurationPerIceCell = 0.5f;

        [Header("消耗")]
        [Tooltip("释放所需能量")]
        public float RequiredEnergy = 65f;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
    }
}
