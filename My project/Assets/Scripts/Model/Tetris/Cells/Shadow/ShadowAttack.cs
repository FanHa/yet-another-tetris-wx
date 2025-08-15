using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class ShadowAttack : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.ShadowAttack;
        public override AffinityType Affinity => AffinityType.Shadow;

        public override string Description()
        {
            return Units.Skills.ShadowAttack.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.ShadowAttack.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.ShadowAttackConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.ShadowAttack(config);
            unit.AddSkill(skillInstance);
        }
    }
}