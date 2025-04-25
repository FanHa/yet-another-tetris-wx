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
        public float distance = 1.5f;

        public override void Apply(Unit unit)
        {
            unit.Attributes.AttackRange += distance;
            unit.Attributes.IsRanged = true;
        }

        public override string Description()
        {
            return "单位转变为远程攻击,攻击距离 +" + distance + ",但伤害会衰减";
        }

        public override string Name()
        {
            return "远程攻击";
        }

        
    }
}