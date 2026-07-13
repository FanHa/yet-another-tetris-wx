using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class EnergyAbsorb : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.EnergyAbsorb;
        public override AffinityType Affinity => AffinityType.Absorb;

        public override string Description()
        {
            return Units.Skills.EnergyAbsorb.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.EnergyAbsorb.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.EnergyAbsorbSkillConfig;
            var config = configGroup?.TryGetLevelConfig(Level, out var levelConfig) == true ? levelConfig : null;
            var skillInstance = new Units.Skills.EnergyAbsorb(config);
            unit.AddSkill(skillInstance);
        }
    }
}