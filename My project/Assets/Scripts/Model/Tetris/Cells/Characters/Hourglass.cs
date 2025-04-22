using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Hourglass : Character
    {
        public float CooldownReductionPercentage = 10f; // 技能冷却时间减少百分比
        public float BuffDurationIncreasePercentage = 10f; // Buff 持续时间增加百分比

        public Hourglass()
        {
            AttackPowerValue = 9f;
            MaxCoreValue = 90f;
        }
        public override void Apply(Unit unit)
        {
            base.Apply(unit);
            // 减少技能冷却时间
            unit.SkillManager.CooldownRevisePercentage = 100 - CooldownReductionPercentage;
            // 增加 Buff 持续时间
            unit.BuffManager.DurationRevisePercentage = 100 + BuffDurationIncreasePercentage;
        }

        public override string Name()
        {
            return "沙漏";
        }

        public override string Description()
        {
            return base.Description()+
                $"减少技能冷却时间 {CooldownReductionPercentage}%，延长 Buff 持续时间 {BuffDurationIncreasePercentage}%";
        }
    }
}