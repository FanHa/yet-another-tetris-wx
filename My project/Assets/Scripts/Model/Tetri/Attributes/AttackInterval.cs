using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class AttackInterval : Attribute
    {
        [SerializeField]
        public float IntervalBonus = 0.3f;

        public override void ApplyAttributes(Unit unit)
        {
            unit.attackCooldown -= IntervalBonus;
        }

        public override string Description()
        {
            return "Attack CoolDown : -" + IntervalBonus;
        }
    }
}