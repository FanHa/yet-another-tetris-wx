using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class IceBreaker : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.IceBreaker;
        public override AffinityType Affinity => AffinityType.Ice;

        public override string Description()
        {
            return Units.Skills.IceBreaker.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.IceBreaker.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.IceBreakerConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.IceBreaker(config);
            unit.AddSkill(skillInstance);
        }
    }
}