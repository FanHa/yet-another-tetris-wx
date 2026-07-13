using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/LifeEchoSkillConfig")]
    public class LifeEchoSkillConfig : SkillConfig<LifeEchoLevelConfig>
    {
    }

    [System.Serializable]
    public class LifeEchoLevelConfig : SkillLevelConfig
    {
        [Header("属性")]
        public float DamagePercentToReflect; // 反射伤害百分比

    }
}
