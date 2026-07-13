using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class WindKnockback : Cell
    {
        // Todo5 完成 CellType 注册后将 None 替换为 CellTypeId.WindKnockback
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
            var configGroup = SkillConfig as Units.Skills.WindKnockbackSkillConfig;
            var config = configGroup?.TryGetLevelConfig(Level, out var levelConfig) == true ? levelConfig : null;
            var skillInstance = new Units.Skills.WindKnockback(config);
            unit.AddSkill(skillInstance);
        }
    }
}
