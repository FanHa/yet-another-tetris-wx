using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class WindShift : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.WindShift;

        public WindShift()
        {
            Affinity = AffinityType.Wind;
        }

        public override string Description()
        {
            return "切换为风形态，提升攻击距离，降低伤害，提升自身受到伤害。";
        }

        public override string Name()
        {
            return "风形态";
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.WindShiftConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.WindShift(config);
            unit.AddSkill(skillInstance);
        }
    }
}