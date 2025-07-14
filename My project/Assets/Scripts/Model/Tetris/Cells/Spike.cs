using System;
using System.Collections.Generic;
using Units;
using Units.Damages;

namespace Model.Tetri
{
    [Serializable]
    public class Spike : Cell
    {
        private float reflectPercentage = 10; // 反弹伤害百分比

        public override void Apply(Unit unit)
        {
            unit.OnDamageTaken += HandleReflectDamage;
        }

        private void HandleReflectDamage(Units.Damages.Damage damage)
        {
            if (damage.Type == DamageType.Hit)
            {
                Units.Damages.Damage reflectDamage = new Units.Damages.Damage(damage.Value * reflectPercentage / 100, DamageType.Skill)
                    .SetSourceLabel(Name())
                    .SetSourceUnit(damage.TargetUnit)
                    .SetTargetUnit(damage.SourceUnit);
                damage.SourceUnit.TakeDamage(reflectDamage);
            }
        }

        public override string Name()
        {
            return "尖刺";
        }

        public override string Description()
        {
            return $"受到攻击伤害时反弹 {reflectPercentage}% 伤害给攻击者";
        }
    }
}