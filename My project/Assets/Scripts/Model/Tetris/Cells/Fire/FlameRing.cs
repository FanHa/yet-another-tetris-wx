using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class FlameRing : Cell
    {
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

            var skillInstance = new Units.Skills.FlameRing();
            skillInstance.SetFireCellCount(fireCellCount);
            unit.AddSkill(skillInstance);
        }
    }
}