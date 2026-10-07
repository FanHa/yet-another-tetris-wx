using UnityEngine;
using Model.Buffs;

namespace Model.Skills
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Skills/Configs/Life/Life Echo")]
    public class LifeEchoSkillConfig : SkillConfig<LifeEchoLevelConfig>
    {
        [Header("Buff")]
        [SerializeField] private BuffDefinition lifeEchoBuffDefinition;

        public BuffDefinition LifeEchoBuffDefinition => lifeEchoBuffDefinition;

    #if UNITY_EDITOR
        protected override void ValidateSpecificConfig()
        {
            ValidateRequiredReference(lifeEchoBuffDefinition, nameof(lifeEchoBuffDefinition));
        }
    #endif
    }

    [System.Serializable]
    public class LifeEchoLevelConfig : SkillLevelConfig
    {
        [Header("属性")]
        public float DamagePercentToReflect; // 反射伤害百分比

    }
}
