using System;
using Model;

namespace Presentation
{
    public enum StatLabel
    {
        AttackSpeedIncreasePercent,
        BurnDamagePerSecond,
        BurnDuration,
        ChillDuration,
        MovementSpeedReductionPercent,
        AttackSpeedReductionPercent,
        ActionRateReductionPercent,
        EnergyPerEnemySkillCast,
        EnergyRegenerationReductionPercent,
        FreezeDuration,
        BaseExtraDamage,
        SlowBasedDamageMultiplier,
        ReflectedDamagePercent,
        MaximumHealth,
        MaxHealthConversionPercent,
        AttackHitExtraDamage,
        AttackReductionPercent,
        VulnerabilityPercent,
        VulnerabilityDuration,
        WildWindDuration,
        WildWindMovementSpeedReductionPercent,
        WildWindAttackReductionPercent,
        KnockbackDistance,
        MaxKnockbackDistance,
        AttackRangeIncrease,
        DomainRadius,
        DomainDuration,
        FirstChainDamage,
        AreaRadius,
        Duration,
        ElectricDamage,
        SkillEnergyReduction,
        SkillDamage,
        AreaDuration,
        HealthCostPercent,
        LifeCostPercent,
        ShieldMultiplierPercent,
        FieldRadius,
        FieldDuration,
        ActionRadius,
        DashDamage,
        Damage,
        WindFieldDuration
    }

    public static class StatBreakdownFormatter
    {
        public static string Format(StatLabel label, AdditiveValueBreakdown value)
        {
            string labelText = GetLabelText(label);
            if (value.AddedAmount == 0 && value.ReducedAmount == 0)
                return $"{labelText}: <final>{value.FinalValue}</final>";

            if (value.AddedAmount != 0 && value.ReducedAmount == 0)
                return $"{labelText}: <final>{value.FinalValue}</final> [<base>{value.BaseValue}</base> + <bonus>{value.AddedAmount}</bonus>]";

            if (value.AddedAmount == 0 && value.ReducedAmount != 0)
                return $"{labelText}: <final>{value.FinalValue}</final> [<base>{value.BaseValue}</base> - <debuff>{value.ReducedAmount}</debuff>]";

            return $"{labelText}: <final>{value.FinalValue}</final> [<base>{value.BaseValue}</base> + <bonus>{value.AddedAmount}</bonus> - <debuff>{value.ReducedAmount}</debuff>]";
        }

        private static string GetLabelText(StatLabel label)
        {
            switch (label)
            {
                case StatLabel.AttackSpeedIncreasePercent: return "攻击速度提升(%)";
                case StatLabel.BurnDamagePerSecond: return "灼烧每秒伤害";
                case StatLabel.BurnDuration: return "灼烧持续时间";
                case StatLabel.ChillDuration: return "减速效果持续时间";
                case StatLabel.MovementSpeedReductionPercent: return "移动速度降低(%)";
                case StatLabel.AttackSpeedReductionPercent: return "攻击速度降低(%)";
                case StatLabel.ActionRateReductionPercent: return "动作速率降低(%)";
                case StatLabel.EnergyPerEnemySkillCast: return "每次敌方施法为每个友军补充能量";
                case StatLabel.EnergyRegenerationReductionPercent: return "能量回复降低(%)";
                case StatLabel.FreezeDuration: return "冻结持续时间";
                case StatLabel.BaseExtraDamage: return "基础额外伤害";
                case StatLabel.SlowBasedDamageMultiplier: return "减速百分比伤害倍率";
                case StatLabel.ReflectedDamagePercent: return "反弹伤害比例(%)";
                case StatLabel.MaximumHealth: return "最大生命";
                case StatLabel.MaxHealthConversionPercent: return "最大生命转换率(%)";
                case StatLabel.AttackHitExtraDamage: return "攻击命中额外伤害";
                case StatLabel.AttackReductionPercent: return "攻击降低(%)";
                case StatLabel.VulnerabilityPercent: return "易伤百分比(%)";
                case StatLabel.VulnerabilityDuration: return "易伤持续时间";
                case StatLabel.WildWindDuration: return "DeBuff持续时间";
                case StatLabel.WildWindMovementSpeedReductionPercent: return "DeBuff移动速度降低(%)";
                case StatLabel.WildWindAttackReductionPercent: return "DeBuff攻击力降低(%)";
                case StatLabel.KnockbackDistance: return "击退距离";
                case StatLabel.MaxKnockbackDistance: return "最大击退距离";
                case StatLabel.AttackRangeIncrease: return "攻击距离提升";
                case StatLabel.DomainRadius: return "领域半径";
                case StatLabel.DomainDuration: return "领域持续";
                case StatLabel.FirstChainDamage: return "首跳伤害";
                case StatLabel.AreaRadius: return "范围半径";
                case StatLabel.Duration: return "持续时间";
                case StatLabel.ElectricDamage: return "电击伤害";
                case StatLabel.SkillEnergyReduction: return "削减技能能量";
                case StatLabel.SkillDamage: return "技能伤害";
                case StatLabel.AreaDuration: return "区域持续时间";
                case StatLabel.HealthCostPercent: return "消耗生命值百分比";
                case StatLabel.LifeCostPercent: return "生命值消耗比例(%)";
                case StatLabel.ShieldMultiplierPercent: return "原始生命消耗转护盾倍率(%)";
                case StatLabel.FieldRadius: return "作用半径";
                case StatLabel.FieldDuration: return "风场持续时间";
                case StatLabel.ActionRadius: return "作用半径";
                case StatLabel.DashDamage: return "冲刺伤害";
                case StatLabel.Damage: return "伤害";
                case StatLabel.WindFieldDuration: return "风场持续时间";
                default: throw new ArgumentOutOfRangeException(nameof(label), label, null);
            }
        }
    }
}