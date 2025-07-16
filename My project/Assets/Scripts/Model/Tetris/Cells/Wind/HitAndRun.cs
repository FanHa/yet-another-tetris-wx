using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class HitAndRun : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.HitAndRun;
        public override AffinityType Affinity => AffinityType.Wind;

        public override string Description()
        {
            return "与敌人保持攻击距离";
        }

        public override string Name()
        {
            return "走A";
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.HitAndRunConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.HitAndRun(config);
            unit.AddSkill(skillInstance);
        }
    }
}