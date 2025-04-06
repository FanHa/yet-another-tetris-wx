using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Speed : Cell, IBaseAttribute
    {
        [SerializeField]
        private int moveSpeedPercentageModifier = 50; // 移动速度百分比修正值

        public void ApplyAttributes(Unit unit)
        {
            unit.moveSpeed.AddPercentageModifier(this, moveSpeedPercentageModifier); // 添加移动速度修正值
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