using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Triangle : Character
    {
        [SerializeField]
        private float damageBoostPercentage = 20f; // 直接表示提升的百分比

        public override string Description()
        {
            // 动态生成描述字符串，反映当前的提升百分比
            return $"{damageBoostPercentage:F1}% damage boost";
        }

        public override void ApplyAttributes(Units.Unit unitComponent)
        {
            // 将百分比转换为倍率并应用
            float multiplier = 1 + (damageBoostPercentage / 100f);
            unitComponent.attackDamage *= multiplier;
        }
    }
}