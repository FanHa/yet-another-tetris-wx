using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Attack : Cell, IBaseAttribute
    {
        [SerializeField]
        private int attackPowerPercentageModifier = 20; // 攻击力百分比修正值


        public void ApplyAttributes(Unit unit)
        {
            unit.attackPower.AddPercentageModifier(this, attackPowerPercentageModifier); // 添加攻击力修正值
        }

        public override string Description()
        {
            return $"攻击力 +{attackPowerPercentageModifier}%";

        }

        public override string Name()
        {
            return "武器";
        }
    }
}