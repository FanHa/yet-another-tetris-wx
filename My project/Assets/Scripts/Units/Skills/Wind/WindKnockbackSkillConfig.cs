using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/WindKnockbackSkillConfig")]
    public class WindKnockbackSkillConfig : SkillConfig<WindKnockbackLevelConfig>
    {
    }

    [System.Serializable]
    public class WindKnockbackLevelConfig : SkillLevelConfig
    {
        [Header("属性")]
        public float BaseKnockbackDistance = 0.12f;
        public float KnockbackDistancePerWindCell = 0.01f;
        public float MaxKnockbackDistance = 0.18f;
    }
}
