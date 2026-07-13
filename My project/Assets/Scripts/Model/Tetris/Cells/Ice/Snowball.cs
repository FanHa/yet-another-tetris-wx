using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class Snowball : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.Snowball;
        public override AffinityType Affinity => AffinityType.Ice;


        public override string Description()
        {
            return Units.Skills.Snowball.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.Snowball.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.SnowballSkillConfig;
            var config = configGroup?.TryGetLevelConfig(Level, out var levelConfig) == true ? levelConfig : null;
            var skillInstance = new Units.Skills.Snowball(config);
            unit.AddSkill(skillInstance);
        }
    }
}