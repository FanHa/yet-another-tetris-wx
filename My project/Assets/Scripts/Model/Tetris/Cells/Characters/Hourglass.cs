using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Hourglass : Character
    {
        [SerializeField] private float attackPowerValue = 9f; // 攻击力参数
        [SerializeField] private float maxCoreValue = 90f;   
        private const float CooldownReductionPercentage = 10f; // 技能冷却时间减少百分比
        private const float BuffDurationIncreasePercentage = 10f; // Buff 持续时间增加百分比

        public override void Apply(Unit unit)
        {
            unit.Attributes.AttackPower.SetBaseValue(attackPowerValue);
            unit.Attributes.MaxHealth.SetBaseValue(maxCoreValue);
            // 减少技能冷却时间
            unit.SkillManager.CooldownRevisePercentage = 100 - CooldownReductionPercentage;
            // 增加 Buff 持续时间
            unit.BuffManager.DurationRevisePercentage = 100 + BuffDurationIncreasePercentage;
            unit.name = CharacterName;
        }

        public override string Name()
        {
            return "沙漏";
        }

        public override string Description()
        {
            return $"基础攻击力：{attackPowerValue}，最大生命值：{maxCoreValue}。" +
                $"减少技能冷却时间 {CooldownReductionPercentage}%，延长 Buff 持续时间 {BuffDurationIncreasePercentage}%。";
        }
    }
}