using UnityEngine;
using Units.Projectiles;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Fire/Flame Ring")]
    public class FlameRingSkillConfig : SkillConfig<FlameRingLevelConfig>
    {
        [Header("资源")]
        [SerializeField]
        private FlameRing projectilePrefab;

        public FlameRing ProjectilePrefab => projectilePrefab;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(projectilePrefab, nameof(projectilePrefab));
        }
    #endif
    }

    [System.Serializable]
    public class FlameRingLevelConfig: SkillLevelConfig
    {
        [Header("区域效果")]
        public float AreaDuration = -1f;

        [Header("范围")]
        public float BaseRadius = 1f;
        public float RadiusPerResonantCell = 0.2f;
    }
}
