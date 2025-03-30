using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Heavy : Cell, IBaseAttribute // 重型属性类
    {
        [SerializeField]
        private int massPercentageModifier = 100; // 质量百分比修正值

        public void ApplyAttributes(Unit unit)
        {
            unit.massPercentageModifiers.Add(massPercentageModifier); // 添加修正值到列表
        }

        public override string Description()
        {
            return $"重量 +{massPercentageModifier}%";
        }
    }
}