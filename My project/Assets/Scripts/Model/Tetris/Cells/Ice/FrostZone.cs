using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class FrostZone : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.FrostZone;
        public override AffinityType Affinity => AffinityType.Ice;

        public override string Description()
        {
            return Units.Skills.FrostZone.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.FrostZone.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.FrostZoneSkillConfig;
            var config = configGroup?.TryGetLevelConfig(Level, out var levelConfig) == true ? levelConfig : null;
            var skillInstance = new Units.Skills.FrostZone(config);
            unit.AddSkill(skillInstance);
        }
    }
}