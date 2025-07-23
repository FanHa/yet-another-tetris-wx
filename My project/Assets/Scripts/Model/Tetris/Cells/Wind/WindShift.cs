using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class WindShift : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.WindShift;
        public override AffinityType Affinity => AffinityType.Wind;

        public override string Description()
        {
            return Units.Skills.WindShift.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.WindShift.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.WindShiftConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.WindShift(config);
            unit.AddSkill(skillInstance);
        }
    }
}