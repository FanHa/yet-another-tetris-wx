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
            var configGroup = SkillConfigGroup as Units.Skills.ChargeConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.Charge(config);
            unit.AddSkill(skillInstance);
        }
    }
}