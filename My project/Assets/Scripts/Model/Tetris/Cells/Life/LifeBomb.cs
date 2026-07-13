using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class LifeBomb : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.LifeBomb;
        public override AffinityType Affinity => AffinityType.Life;

        public override string Description()
        {
            return Units.Skills.LifeBomb.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.LifeBomb.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.LifeBombSkillConfig;
            var config = configGroup?.GetLevelConfig(Level);
            var skillInstance = new Units.Skills.LifeBomb(config);
            unit.AddSkill(skillInstance);
        }
    }
}