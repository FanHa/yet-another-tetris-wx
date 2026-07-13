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
            var configGroup = SkillConfig as Units.Skills.WindShiftSkillConfig;
            var config = configGroup?.GetLevelConfig(Level);
            var skillInstance = new Units.Skills.WindShift(config);
            unit.AddSkill(skillInstance);
        }
    }
}