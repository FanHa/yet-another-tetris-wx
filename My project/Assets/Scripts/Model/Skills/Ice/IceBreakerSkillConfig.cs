using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Ice Breaker")]
    public class IceBreakerSkillConfig : SkillConfig<IceBreakerLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private BuffDefinition iceBreakerBuffDefinition;

        public BuffDefinition IceBreakerBuffDefinition => iceBreakerBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(iceBreakerBuffDefinition, nameof(iceBreakerBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class IceBreakerLevelConfig : SkillLevelConfig
    {
        [Header("通用")]
        public float BuffDuration;

        [Header("属性")]
        public float BaseExtraDamage;
        public float MultiplierByChilledLayer;

        [Header("加成")]
        public float ExtraDamagePerIceCell;

    }
}
