using UnityEngine;
using UnityEngine.Serialization;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Shadow/Shadow Arrow")]
    public class ShadowArrowSkillConfig : SkillConfig<ShadowArrowLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private ShadowArrow projectilePrefab;
        [SerializeField] private BuffDefinition vulnerabilityBuffDefinition;

        public ShadowArrow ProjectilePrefab => projectilePrefab;
        public BuffDefinition VulnerabilityBuffDefinition => vulnerabilityBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(vulnerabilityBuffDefinition, nameof(vulnerabilityBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class ShadowArrowLevelConfig : SkillLevelConfig, IRequiresEnergy
    {
        public float RequiredEnergy ;
        float IRequiresEnergy.RequiredEnergy => RequiredEnergy;

        public float Damage;
        public float DamagePerResonantCell;
        
        public float VulnerabilityPercent;
        public float VulnerabilityPercentPerResonantCell;
        public float VulnerabilityDuration;
    }
}
