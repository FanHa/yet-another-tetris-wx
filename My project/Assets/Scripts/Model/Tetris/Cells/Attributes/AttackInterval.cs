using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class AttackInterval : Attribute
    {
        [SerializeField]
        public float IntervalBonus = 0.5f;

        public override void ApplyAttributes(Unit unit)
        {
            unit.attackCooldown -= IntervalBonus;
            if (unit.attackCooldown < 0.1f)
            {
                unit.attackCooldown = 0.1f; // 限制最小攻击间隔
            }
        }

        public override string Description()
        {
            return "Attack CoolDown : -" + IntervalBonus;
        }
    }
}