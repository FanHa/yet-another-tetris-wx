using UnityEngine;
using Presentation;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Power")]
    public sealed class LifePowerBuffDefinition : BuffDefinition
    {
        [SerializeField] private float healthToAtkPercent;
        [SerializeField] private float healthToAtkPercentPerResonantCell;

        public AdditiveValueBreakdown CalculateHealthToAttackPercent(int resonantCellCount) =>
            new(healthToAtkPercent, resonantCellCount * healthToAtkPercentPerResonantCell);

        public float CalculateExtraDamage(float casterMaxHealth, int resonantCellCount) =>
            casterMaxHealth * (CalculateHealthToAttackPercent(resonantCellCount).FinalValue / 100f);

        public string DescribeEffect(int resonantCellCount)
        {
            var healthConversionPercent = CalculateHealthToAttackPercent(resonantCellCount);
            return
                "按施法时施法者的最大生命值计算攻击命中附加伤害，不消耗生命值。\n" +
                StatBreakdownFormatter.Format(StatLabel.MaxHealthConversionPercent, healthConversionPercent) + "\n" +
                BaseDuration.ToDisplayString("效果持续时间");
        }

        public string DescribeAppliedEffect(float extraDamage, int resonantCellCount) =>
            DescribeEffect(resonantCellCount) + "\n" +
            StatBreakdownFormatter.Format(StatLabel.AttackHitExtraDamage, new AdditiveValueBreakdown(extraDamage));

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (healthToAtkPercent < 0f || float.IsNaN(healthToAtkPercent) || float.IsInfinity(healthToAtkPercent))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(healthToAtkPercent)} must be finite and non-negative.", this);
            }

            if (healthToAtkPercentPerResonantCell < 0f || float.IsNaN(healthToAtkPercentPerResonantCell) || float.IsInfinity(healthToAtkPercentPerResonantCell))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(healthToAtkPercentPerResonantCell)} must be finite and non-negative.", this);
            }
        }
#endif
    }
}