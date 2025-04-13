using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Speed : Attribute
    {
        [SerializeField]
        private int moveSpeedPercentageModifier = 50; // 移动速度百分比修正值

        public override void Apply(Unit unit)
        {
            unit.Attributes.MoveSpeed.AddPercentageModifier(this, moveSpeedPercentageModifier); // 添加移动速度修正值
        }

        public override string Description()
        {
            return $"移动速度 +{moveSpeedPercentageModifier}%";
        }

        public override string Name()
        {
            return "兵贵神速";
        }
    }
}