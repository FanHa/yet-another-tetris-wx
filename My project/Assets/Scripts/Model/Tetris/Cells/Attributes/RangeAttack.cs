 using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class RangeAttack : Attribute
    {
        [SerializeField]
        public float distance = 3;

        public override void Apply(Unit unit)
        {
            unit.Attributes.AttackRange += distance;
            unit.Attributes.IsRanged = true;
        }

        public override string Description()
        {
            return "攻击距离 +" + distance;
        }

        public override string Name()
        {
            return "技能: 远程攻击";
        }

        
    }
}