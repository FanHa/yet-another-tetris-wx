using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class IcyCage : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.IcyCage;
        public override AffinityType Affinity => AffinityType.Ice;

        public override string Description()
        {
            return "对一个敌人施加冰牢（冻结/极强减速），持续时间随冰系Cell数量提升。";
        }

        public override string Name()
        {
            return "冰牢";
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.IcyCageConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.IcyCage(config);
            unit.AddSkill(skillInstance);
        }
    }
}