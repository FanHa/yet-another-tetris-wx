using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Aim : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Aim;
        public float AttackRange = 2f;

        public override void Apply(Unit unit)
        {
            base.Apply(unit);
            unit.Attributes.AttackRange.SetBaseValue(AttackRange);
            unit.Attributes.AttackRange.AddPercentageModifier(this, 20); // 20%攻击范围加成

            unit.Attributes.MaxHealth.AddPercentageModifier(this, -50); // -50%生命上限
        }

        public override string Name()
        {
            return "小瞄";
        }

        public override string Description()
        {
            return "天生远程单位,更多的攻击距离加成,更少的生命";
        }
    }
}