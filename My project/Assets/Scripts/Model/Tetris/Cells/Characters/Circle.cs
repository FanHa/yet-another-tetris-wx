using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Circle : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Circle;

        public float HealthBonusPercent = 80f;
        public float AttackRangeBonusPercent = -40f;
        public float AttackPowerBonusPercent = -20f; // -20%攻击力
        public float MoveSpeedBonusPercent = -20f;   // -20%速度
        public override void Apply(Unit unit)
        {
            base.Apply(unit);
            unit.Attributes.MaxHealth.AddPercentageModifier(this, HealthBonusPercent);
            unit.Attributes.AttackRange.AddPercentageModifier(this, AttackRangeBonusPercent);
            unit.Attributes.AttackPower.AddPercentageModifier(this, AttackPowerBonusPercent);
            unit.Attributes.MoveSpeed.AddPercentageModifier(this, MoveSpeedBonusPercent);
        }
        public override string Name()
        {
            return "小圆";
        }

        public override string Description()
        {
            return "血量极高，射程、攻击力和速度均较低。";
        }


    }
}