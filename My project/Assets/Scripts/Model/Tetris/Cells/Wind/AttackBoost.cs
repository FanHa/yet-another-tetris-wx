using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class AttackBoost : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.AttackBoost;
        public override AffinityType Affinity => AffinityType.Wind;

        public override string Description()
        {
            return Units.Skills.AttackBoost.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.AttackBoost.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.AttackBoostConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.AttackBoost(config);
            unit.AddSkill(skillInstance);
        }
    }
}