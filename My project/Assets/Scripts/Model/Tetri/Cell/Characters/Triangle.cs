using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Triangle : Character
    {
        [SerializeField]
        private float damageBoostPercentage = 30f; // 直接表示提升的百分比

        [SerializeField]
        private float healthReductionPercentage = 40f; // 表示降低的百分比

        public override string Description()
        {
            // 动态生成描述字符串，反映当前的提升和降低百分比
            return $"{damageBoostPercentage:F1}% damage boost, {healthReductionPercentage:F1}% health reduction";
        }

        public override void ApplyFeatures(Units.Unit unitComponent)
        {
            // 增加伤害
            float damageMultiplier = 1 + (damageBoostPercentage / 100f);
            unitComponent.attackDamage *= damageMultiplier;

            // 降低血量
            float healthMultiplier = 1 - (healthReductionPercentage / 100f);
            unitComponent.maxHP *= healthMultiplier;
        }
    }
}