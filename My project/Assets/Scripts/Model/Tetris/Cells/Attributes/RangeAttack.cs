 using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class RangeAttack : Cell, IBaseAttribute // 远程攻击属性类
    {
        [SerializeField]
        public float distance = 3;

        public void ApplyAttributes(Unit unit)
        {
            unit.attackRange += distance;
            unit.isRanged = true;
        }

        public override string Description()
        {
            return "Attack Range: + " + distance;
        }

        
    }
}