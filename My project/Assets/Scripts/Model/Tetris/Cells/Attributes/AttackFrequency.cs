using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class AttackFrequency : Attribute
    {
        [SerializeField]
        public float AttackSpeedBonusPercentage = 20f;

        public override void Apply(Unit unit)
        {
            unit.Attributes.AttacksPerTenSeconds.AddPercentageModifier(this, AttackSpeedBonusPercentage);
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