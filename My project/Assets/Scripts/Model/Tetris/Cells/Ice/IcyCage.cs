using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class IcyCage : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.IcyCage;
        public IcyCage()
        {
            Affinity = AffinityType.Ice;
        }

        public override string Description()
        {
            return "对一个敌人施加冰牢（冻结/极强减速），持续时间随冰系Cell数量提升。";
        }

        public override string Name()
        {
            return "冰牢";
        }

        public override void PostApply(Unit unit, IReadOnlyList<Cell> allCells)
        {
            int iceCount = 0;
            foreach (var cell in allCells)
            {
                if (cell.Affinity == AffinityType.Ice)
                    iceCount++;
            }
            var configGroup = SkillConfigGroup as Units.Skills.IcyCageConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.IcyCage(config);
            skillInstance.SetIceCellCount(iceCount);
            unit.AddSkill(skillInstance);
        }
    }
}