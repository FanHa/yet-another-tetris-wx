using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class AttackInterval : Cell, IBaseAttribute
    {
        [SerializeField]
        public float AttackSpeedBonusPercentage = 20f;

        public void ApplyAttributes(Unit unit)
        {
            // 增加百分比攻速
            unit.attacksPerTenSeconds.AddPercentageModifier(this, AttackSpeedBonusPercentage);
        }

        public override string Description()
        {
            return "Attack Speed: +" + AttackSpeedBonusPercentage + "%";
        }
    }
}