using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class FlameRing : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameRing;

        public FlameRing()
        {
            Affinity = AffinityType.Fire;
        }

        public override string Description()
        {
            return "每次Tick对周围一圈敌人施加灼烧。";
        }

        public override string Name()
        {
            return "火环";
        }

        public override void PostApply(Unit unit, IReadOnlyList<Cell> allCells)
        {
            int fireCellCount = 0;
            foreach (var cell in allCells)
            {
                if (cell.Affinity == AffinityType.Fire)
                    fireCellCount++;
            }

            var configGroup = SkillConfigGroup as Units.Skills.FlameRingConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.FlameRing(config);
            skillInstance.SetFireCellCount(fireCellCount);
            unit.AddSkill(skillInstance);
        }
    }
}