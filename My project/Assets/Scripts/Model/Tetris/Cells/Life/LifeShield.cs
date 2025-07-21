using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class LifeShield : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeShield;
        public override AffinityType Affinity => AffinityType.Life;

        public override string Description()
        {
            return Units.Skills.LifeShield.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.LifeShield.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.LifeShieldConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.LifeShield(config);
            unit.AddSkill(skillInstance);
        }
    }
}