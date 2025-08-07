using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/LifeEchoConfigGroup")]
    public class LifeEchoConfigGroup : SkillConfigGroup
    {
        public List<LifeEchoConfig> LevelConfigs;
    }

    [System.Serializable]
    public class LifeEchoConfig : SkillConfig
    {
        [Header("属性")]
        public float DamagePercentToReflect; // 反射伤害百分比

    }
}
