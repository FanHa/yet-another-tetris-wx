using UnityEngine;
using Presentation;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Echo")]
    public sealed class LifeEchoBuffDefinition : BuffDefinition
    {
        [SerializeField] private float reflectPercent;
        [SerializeField] private float reflectPercentPerResonantCell;

        public float ReflectPercent => reflectPercent;

        public AdditiveValueBreakdown CalculateReflectPercent(int resonantCellCount) =>
            new(reflectPercent, resonantCellCount * reflectPercentPerResonantCell);

        public string DescribeEffect(int resonantCellCount)
        {
            var reflectPercentBreakdown = CalculateReflectPercent(resonantCellCount);
            string formattedReflectPercent = StatBreakdownFormatter.Format(
                StatLabel.ReflectedDamagePercent,
                reflectPercentBreakdown);

            return
                "受到普攻或技能伤害后，将部分伤害反弹给非自身的来源单位。\n" +
                $"{formattedReflectPercent}\n" +
                BaseDuration.ToDisplayString("效果持续时间");
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