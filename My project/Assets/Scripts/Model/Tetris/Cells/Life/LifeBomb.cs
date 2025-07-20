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
            var configGroup = SkillConfigGroup as Units.Skills.LifeBombConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.LifeBomb(config);
            unit.AddSkill(skillInstance);
        }
    }
}