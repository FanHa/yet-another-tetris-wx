using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class WindKnockback : Cell
    {
        // Todo5 完成 CellType 注册后将 None 替换�?CellTypeId.WindKnockback
        public override CellTypeId CellTypeId => CellTypeId.WindKnockback;
        public override AffinityType Affinity => AffinityType.Wind;

        public override string Description()
        {
            return Units.Skills.WindKnockback.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.WindKnockback.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = Config as Units.Skills.WindKnockbackSkillConfig;
            var config = configGroup?.GetLevelConfig(Level);
            var skillInstance = new Units.Skills.WindKnockback(config);
            unit.AddSkill(skillInstance);
        }
    }
}

