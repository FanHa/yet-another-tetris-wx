using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Triangle : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Triangle;
        public float AttackPowerBonusPercent = 30f;   // +30%攻击力
        public float MaxHealthBonusPercent = -40f;    // -40%生命值
        public float MoveSpeedBonusPercent = 25f;     // +25%移动速度
        
        public override void Apply(Unit unit)
        {
            base.Apply(unit);
            unit.Attributes.AttackPower.AddPercentageModifier(this, AttackPowerBonusPercent);
            unit.Attributes.MaxHealth.AddPercentageModifier(this, MaxHealthBonusPercent);
            unit.Attributes.MoveSpeed.AddPercentageModifier(this, MoveSpeedBonusPercent);
        }
        public override string Name()
        {
            return "小三";
        }

        public override string Description()
        {
            return "攻击力极高，生命值较低，移动速度较快。";
        }
    }
}