using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Hourglass : Character
    {
        private const float CooldownReductionPercentage = 10f; // 技能冷却时间减少百分比
        private const float BuffDurationIncreasePercentage = 10f; // Buff 持续时间增加百分比

        public override void Apply(Unit unit)
        {
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
            return $"减少 {CooldownReductionPercentage}% 的技能冷却时间，增加 {BuffDurationIncreasePercentage}% 的 Buff 持续时间。";
        }
    }
}