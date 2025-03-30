using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Attack : Cell, IBaseAttribute
    {
        [SerializeField]
        private int attackPercentageModifier = 20; // 攻击力百分比修正值


        public void ApplyAttributes(Unit unit)
        {
            unit.attackPowerPercentageModifiers.Add(attackPercentageModifier);
        }

        public override string Description()
        {
            return $"Attack Power Bonus: {attackPercentageModifier}%";

        }
    }
}