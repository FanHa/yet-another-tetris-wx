using System;
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

        private void HandleReflectDamage(Units.Damages.EventArgs args)
        {
            if (args.Damage.CanBeReflected)
            {
                var reflectDamage = new Damage(args.Damage.Value * reflectPercentage / 100, "尖刺", false);
                args.Source.TakeDamage(args.Target, reflectDamage);
            }
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