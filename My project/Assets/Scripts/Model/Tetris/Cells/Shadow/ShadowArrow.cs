using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class ShadowArrow : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.ShadowArrow;
        public override AffinityType Affinity => AffinityType.Shadow;

        public override string Description()
        {
            return Units.Skills.ShadowArrow.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.ShadowArrow.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.ShadowArrowConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.ShadowArrow(config);
            unit.AddSkill(skillInstance);
        }
    }
}