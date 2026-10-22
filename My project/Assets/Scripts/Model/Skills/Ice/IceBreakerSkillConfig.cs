using UnityEngine;
using UnityEngine.Serialization;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Ice/Ice Breaker")]
    public class IceBreakerSkillConfig : SkillConfig<IceBreakerLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private IceBreakerBuffDefinition iceBreakerBuffDefinition;

        public IceBreakerBuffDefinition IceBreakerBuffDefinition => iceBreakerBuffDefinition;

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
    }
}
