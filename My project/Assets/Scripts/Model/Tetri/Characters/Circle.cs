using System;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Circle : Character
    {
        [SerializeField]
        private float healthBoostPercentage = 100f; // 直接表示提升的百分比

        [SerializeField]
        private float damageReductionPercentage = 50f; // 表示降低的百分比

        public override string Description()
        {
            // 动态生成描述字符串，反映当前的提升和降低百分比
            return $"{healthBoostPercentage:F1}% health boost, {damageReductionPercentage:F1}% damage reduction";
        }

        public override void ApplyAttributes(Units.Unit unitComponent)
        {
            // 增加血量
            float healthMultiplier = 1 + (healthBoostPercentage / 100f);
            unitComponent.maxHP *= healthMultiplier;

            // 降低伤害
            float damageMultiplier = 1 - (damageReductionPercentage / 100f);
            unitComponent.attackDamage *= damageMultiplier;
        }
    }
}