using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class ShadowStep : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.ShadowStep;
        public override AffinityType Affinity => AffinityType.Shadow;

        public override string Description()
        {
            return Units.Skills.ShadowStep.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.ShadowStep.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.ShadowStepConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.ShadowStep(config);
            unit.AddSkill(skillInstance);
        }
    }
}