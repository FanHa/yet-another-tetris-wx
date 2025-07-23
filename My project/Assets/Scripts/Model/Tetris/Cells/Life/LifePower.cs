using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class LifePower : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.LifePower;
        public override AffinityType Affinity => AffinityType.Life;

        public override string Description()
        {
            return Units.Skills.LifePower.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.LifePower.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.LifePowerConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.LifePower(config);
            unit.AddSkill(skillInstance);
        }
    }
}