using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Speed : Cell, IBaseAttribute
    {
        [SerializeField]
        private int moveSpeedPercentageModifier = 100; // 移动速度百分比修正值

        public void ApplyAttributes(Unit unit)
        {
            unit.moveSpeedPercentageModifiers.Add(moveSpeedPercentageModifier);
        }

        public override string Description()
        {
            return $"Speed Bonus: {moveSpeedPercentageModifier}%";
        }
    }
}