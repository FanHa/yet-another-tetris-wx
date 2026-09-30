using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Shadow/Shadow Arrow")]
    public class ShadowArrowSkillConfig : SkillConfig<ShadowArrowLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private ShadowArrow projectilePrefab;

        public ShadowArrow ProjectilePrefab => projectilePrefab;
    }

    [System.Serializable]
    public class ShadowArrowLevelConfig : SkillLevelConfig
    {
        public float RequiredEnergy ;

        public float Damage;
        public float DamagePerShadowCell;
        
        public float VulnerabilityPercent;
        public float VulnerabilityPercentPerShadowCell;
        public float VulnerabilityDuration;
    }
}
