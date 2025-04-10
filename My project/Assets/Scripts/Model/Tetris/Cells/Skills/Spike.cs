using System;
using Units;
using Units.Damages;
namespace Model.Tetri.Skills
{
    [Serializable]
    public class Spike : Cell, IBaseAttribute
    {
        private float reflectPercentage = 10; // 反弹伤害百分比

        public void ApplyAttributes(Unit unit)
        {
            unit.CanReflectDamage = true; // 允许反弹伤害
            unit.ReflectDamagePercentage = reflectPercentage; // 设置反弹伤害百分比
        }

        public override string Name()
        {
            return "技能: 尖刺";
        }

        public override string Description()
        {
            return $"受到伤害时反弹 {reflectPercentage}% 伤害给攻击者";
        }

    }
}