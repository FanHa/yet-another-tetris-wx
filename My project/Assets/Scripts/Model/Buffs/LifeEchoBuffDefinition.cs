using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Echo")]
    public sealed class LifeEchoBuffDefinition : BuffDefinition
    {
        [SerializeField] private float reflectPercent;
        [SerializeField] private float reflectPercentPerResonantCell;

        public float ReflectPercent => reflectPercent;

        public (StatValue ReflectPercent, string Duration) CalculateStats(int resonantCellCount) =>
            (
            new StatValue("反弹伤害比例(%)", reflectPercent, resonantCellCount * reflectPercentPerResonantCell),
                BaseDuration.ToDisplayString("效果持续时间")
            );

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"受到普攻或技能伤害后，将部分伤害反弹给非自身的来源单位。\n{stats.ReflectPercent}\n{stats.Duration}";
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (reflectPercent < 0f || float.IsNaN(reflectPercent) || float.IsInfinity(reflectPercent))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(reflectPercent)} must be finite and non-negative.", this);
            }

            if (reflectPercentPerResonantCell < 0f || float.IsNaN(reflectPercentPerResonantCell) || float.IsInfinity(reflectPercentPerResonantCell))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(reflectPercentPerResonantCell)} must be finite and non-negative.", this);
            }
        }
#endif
    }
}