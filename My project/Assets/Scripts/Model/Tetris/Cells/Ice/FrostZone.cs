using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class FrostZone : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.FrostZone;
        public override AffinityType Affinity => AffinityType.Ice;

        public override string Description()
        {
            return "在目标区域生成霜域，对范围内敌人造成冰属性伤害并施加冰霜减速。";
        }

        public override string Name()
        {
            return "霜域";
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.FrostZoneConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.FrostZone(config);
            unit.AddSkill(skillInstance);
        }
    }
}