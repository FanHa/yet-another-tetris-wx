using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Echo")]
    public sealed class LifeEchoBuffDefinition : BuffDefinition
    {
        [SerializeField] private float duration = -1f;
        [SerializeField] private float reflectPercent;
        [SerializeField] private float reflectPercentPerResonantCell;

        public float Duration => duration;
        public float ReflectPercent => reflectPercent;

        public (StatValue ReflectPercent, StatValue Duration) CalculateStats(int resonantCellCount) =>
            (
            new StatValue("反弹伤害比例(%)", reflectPercent, resonantCellCount * reflectPercentPerResonantCell),
                new StatValue("效果持续时间", duration)
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
            if (float.IsNaN(duration) || float.IsInfinity(duration) || (duration != -1f && duration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be -1 (permanent) or finite and greater than zero.", this);
            }

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