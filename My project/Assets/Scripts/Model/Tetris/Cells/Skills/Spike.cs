using System;
using Units;
namespace Model.Tetri
{
    [Serializable]
    public class Spike : Cell, IBaseAttribute, ITakeDamageBehavior
    {
        private float reflectPercentage = 0.15f; // 反弹伤害百分比

        public void ApplyAttributes(Unit unit)
        {
            unit.AddDamageBehavior(this);
        }

        public override string Description()
        {
            return $"技能: 尖刺. 受到伤害时反弹 {reflectPercentage * 100}% 伤害给攻击者";
        }

        public float ModifyDamage(Unit source, float damage)
        {
            if (source != null)
            {
                source.TakeDamage(null, damage * reflectPercentage); // 反弹伤害
            }
            return damage;
        }
    }
}