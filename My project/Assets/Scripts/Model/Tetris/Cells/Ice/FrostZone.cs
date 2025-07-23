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
            var configGroup = SkillConfigGroup as Units.Skills.FrostZoneConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.FrostZone(config);
            unit.AddSkill(skillInstance);
        }
    }
}