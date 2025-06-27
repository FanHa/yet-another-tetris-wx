using System.Collections.Generic;
using UnityEngine;

namespace Units.Skills
{
    [CreateAssetMenu(menuName = "SkillConfig/FireballConfigGroup")]
    public class FireballConfigGroup : SkillConfigGroup
    {
        public List<FireballConfig> LevelConfigs;
    }

    [System.Serializable]
    public class FireballConfig : SkillConfig
    {
        [Header("伤害")]
        public float BaseDamage = 20f;
        public float FireCellDamageBonus = 5f;

        [Header("灼烧效果")]
        public float DotPerFireCell = 1f;
        public float DotDurationPerFireCell = 1f;
        public float DotBaseDuration = 3f;

        [Header("消耗")]
        public float RequiredEnergy = 100f;
    }
}
