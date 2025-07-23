using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class WildWind : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.WildWind;
        public override AffinityType Affinity => AffinityType.Wind;

        public override string Description()
        {
            return Units.Skills.WildWind.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.WildWind.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.WildWindConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.WildWind(config);
            unit.AddSkill(skillInstance);
        }
    }
}