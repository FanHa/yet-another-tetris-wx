using UnityEngine;
using Units.Projectiles;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Fire/Flame Ring")]
    public class FlameRingSkillConfig : SkillConfig<FlameRingLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private FlameRing projectilePrefab;
        [SerializeField] private BuffDefinition flameRingBuffDefinition;
        [SerializeField] private BuffDefinition burnBuffDefinition;

        public FlameRing ProjectilePrefab => projectilePrefab;
        public BuffDefinition FlameRingBuffDefinition => flameRingBuffDefinition;
        public BuffDefinition BurnBuffDefinition => burnBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
            ValidateRequiredReference(flameRingBuffDefinition, nameof(flameRingBuffDefinition));
            ValidateRequiredReference(burnBuffDefinition, nameof(burnBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class FlameRingLevelConfig: SkillLevelConfig
    {
        [Header("灼烧效果")]
        public float BaseDotDps = 2f;
        public float DotDpsPerFireCell = 1f;
        public float BaseDotDuration = 2f;
        public float DotDurationPerFireCell = 1f;

        [Header("Buff")]
        public float BuffDuration = -1f;

        [Header("范围")]
        public float BaseRadius = 1f;
        public float RadiusPerFireCell = 0.2f;
    }
}
