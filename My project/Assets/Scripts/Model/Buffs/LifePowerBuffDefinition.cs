using UnityEngine;
using Units.Skills;

namespace Model.Buffs
{
    [CreateAssetMenu(menuName = "Game Data/Gameplay/Buffs/Definitions/Life Power")]
    public sealed class LifePowerBuffDefinition : BuffDefinition
    {
        [SerializeField] private float duration;
        [SerializeField] private float healthToAtkPercent;
        [SerializeField] private float healthToAtkPercentPerResonantCell;

        public float Duration => duration;

        public (StatValue HealthToAtkPercent, StatValue Duration) CalculateStats(int resonantCellCount) =>
            (
            new StatValue("最大生命转换率(%)", healthToAtkPercent, resonantCellCount * healthToAtkPercentPerResonantCell),
                new StatValue("效果持续时间", duration)
            );

        public float CalculateExtraDamage(float casterMaxHealth, int resonantCellCount) =>
            casterMaxHealth * (CalculateStats(resonantCellCount).HealthToAtkPercent.Final / 100f);

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = CalculateStats(resonantCellCount);
            return $"按施法时施法者的最大生命值计算攻击命中附加伤害，不消耗生命值。\n{stats.HealthToAtkPercent}\n{stats.Duration}";
        }

        public string DescribeEffect(float extraDamage, int resonantCellCount) =>
            $"{DescribeEffect(resonantCellCount)}\n{new StatValue("攻击命中额外伤害", extraDamage)}";

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (float.IsNaN(duration) || float.IsInfinity(duration) || (duration != -1f && duration <= 0f))
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(duration)} must be -1 (permanent) or finite and greater than zero.", this);
            }

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