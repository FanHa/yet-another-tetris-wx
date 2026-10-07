using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Shadow/Shadow Step")]
    public class ShadowStepSkillConfig : SkillConfig<ShadowStepLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private BuffDefinition vulnerabilityBuffDefinition;

        public BuffDefinition VulnerabilityBuffDefinition => vulnerabilityBuffDefinition;

#if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(vulnerabilityBuffDefinition, nameof(vulnerabilityBuffDefinition));
            foreach (var (i, level) in GetValidLevelConfigs())
            {
                ValidateFiniteNonNegative(level.InitEnergy, $"{nameof(LevelConfigs)}[{i}].{nameof(level.InitEnergy)}");

                if (level.InitEnergy > level.RequiredEnergy)
                {
                    Debug.LogError($"{nameof(level.InitEnergy)} must be between 0 and {nameof(level.RequiredEnergy)}.", this);
                }
            }
        }
#endif
    }

    [System.Serializable]
    public class ShadowStepLevelConfig : SkillLevelConfig, IRequiresEnergy
    {

        [Header("消耗")]
        public float RequiredEnergy;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;
        public float InitEnergy;

        public float VulnerabilityPercent;
        public float VulnerabilityPercentPerShadowCell;
        public float Damage;
        public float DamagePerShadowCell;

        public float DebuffDuration;
    }
}
