using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class FrostZone : Cell
    {
        public FrostZone()
        {
            Affinity = AffinityType.Ice;
        }

        public override string Description()
        {
            return "在目标区域生成霜域，对范围内敌人造成冰属性伤害并施加冰霜减速。";
        }

        public override string Name()
        {
            return "霜域";
        }

        public override void PostApply(Unit unit, IReadOnlyList<Cell> allCells)
        {
            int iceCount = 0;
            foreach (Model.Tetri.Cell cell in allCells)
            {
                if (cell.Affinity == AffinityType.Ice)
                    iceCount++;
            }

            var skillInstance = new Units.Skills.FrostZone();
            skillInstance.SetIceCellCount(iceCount);
            unit.AddSkill(skillInstance);
        }
    }
}