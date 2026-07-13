using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class LifeEcho : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeEcho;
        public override AffinityType Affinity => AffinityType.Life;

        public override string Description()
        {
            return Units.Skills.LifeEcho.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.LifeEcho.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.LifeEchoSkillConfig;
            var config = configGroup?.TryGetLevelConfig(Level, out var levelConfig) == true ? levelConfig : null;
            var skillInstance = new Units.Skills.LifeEcho(config);
            unit.AddSkill(skillInstance);
        }
    }
}