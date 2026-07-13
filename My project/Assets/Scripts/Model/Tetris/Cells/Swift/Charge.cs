using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class Charge : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.Charge;
        public override AffinityType Affinity => AffinityType.Swift;

        public override string Description()
        {
            return Units.Skills.Charge.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.Charge.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.ChargeSkillConfig;
            var config = configGroup?.TryGetLevelConfig(Level, out var levelConfig) == true ? levelConfig : null;
            var skillInstance = new Units.Skills.Charge(config);
            unit.AddSkill(skillInstance);
        }
    }
}