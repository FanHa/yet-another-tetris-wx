using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class IcyCage : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.IcyCage;
        public override AffinityType Affinity => AffinityType.Ice;

        public override string Description()
        {
            return Units.Skills.IcyCage.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.IcyCage.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.IcyCageSkillConfig;
            var config = configGroup?.TryGetLevelConfig(Level, out var levelConfig) == true ? levelConfig : null;
            var skillInstance = new Units.Skills.IcyCage(config);
            unit.AddSkill(skillInstance);
        }
    }
}