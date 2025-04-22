using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Aim : Character
    {
        public float AttackRange = 2f;
        public Aim()
        {
            AttackPowerValue = 9f;
            MaxCoreValue = 90f;
            RangeAttackDamagePercentage = 100f;
            IsRanged = true;
        }
        public override void Apply(Unit unit)
        {
            base.Apply(unit);
            unit.Attributes.AttackRange = AttackRange;
        }

        public override string Name()
        {
            return "小瞄";
        }

        public override string Description()
        {
            return "天生远程单位,且没有远程攻击伤害衰减" + base.Description();
        }
    }
}