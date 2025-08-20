using System;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public class Hourglass : Character
    {
        public override CharacterTypeId CharacterTypeId => CharacterTypeId.Hourglass;

        public float MaxHealthBonusPercent = -30f;      // -30%生命值
        public float EnergyRegenBonusPercent = 50f;     // +50%能量回复

        public Hourglass()
        {
            AttackPowerValue = 10f;
            MaxCoreValue = 80f;
        }

        public override void Apply(Unit unit)
        {
            base.Apply(unit);
            unit.Attributes.MaxHealth.AddPercentageModifier(this, MaxHealthBonusPercent);
            unit.Attributes.EnergyPerSecond.AddPercentageModifier(this, EnergyRegenBonusPercent);
        }

        public override string Name()
        {
            return "沙漏";
        }

        public override string Description()
        {
            return "生命值较低，但能量回复更高。";
        }
    }
}