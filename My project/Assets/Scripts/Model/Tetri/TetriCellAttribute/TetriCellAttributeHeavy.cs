 using System;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class TetriCellAttributeHeavy : TetriCellAttribute
    {
        [SerializeField]
        public float multiMass = 2;

        public override void ApplyAttributes(Unit unit)
        {
            // 获取Unit的Rigidbody2D组件
            Rigidbody2D rb = unit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 修改mass属性，乘以multiMass
                rb.mass *= multiMass;
            }
        }

        public override string Description()
        {
            return "Heavy: *" + multiMass;
        }
    }
}