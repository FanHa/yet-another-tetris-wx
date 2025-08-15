using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/ShadowAttackConfigGroup")]
    public class ShadowAttackConfigGroup : SkillConfigGroup
    {
        public List<ShadowAttackConfig> LevelConfigs;
    }

    [System.Serializable]
    public class ShadowAttackConfig : SkillConfig
    {

        [Header("消耗")]
        public float RequiredEnergy;
        public float InitEnergy;

        public int WeakLevel;
        public float Damage;
    }
}
