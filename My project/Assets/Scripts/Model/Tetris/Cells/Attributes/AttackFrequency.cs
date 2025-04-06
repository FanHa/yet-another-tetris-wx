using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class AttackFrequency : Cell, IBaseAttribute
    {
        [SerializeField]
        public float AttackSpeedBonusPercentage = 20f;

        public void ApplyAttributes(Unit unit)
        {
            unit.attacksPerTenSeconds.AddPercentageModifier(this, AttackSpeedBonusPercentage);
        }

        public override string Description()
        {
            return "攻击速度: +" + AttackSpeedBonusPercentage + "%";
        }

        public override string Name()
        {
            return "攻速";
        }
    }
}