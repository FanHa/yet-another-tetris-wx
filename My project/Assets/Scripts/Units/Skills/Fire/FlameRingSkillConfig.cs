using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/FlameRingSkillConfig")]
    public class FlameRingSkillConfig : SkillConfig<FlameRingLevelConfig>
    {
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
