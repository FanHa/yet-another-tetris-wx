using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class Snowball : Cell
    {
        public Snowball()
        {
            Affinity = AffinityType.Ice;
        }

        public override string Description()
        {
            return "发射一个雪球，造成冰属性伤害并施加Chilled（冰霜减速Debuff）。";
        }

        public override string Name()
        {
            return "雪球";
        }

        public override void PostApply(Unit unit, IReadOnlyList<Cell> allCells)
        {
            int iceCount = 0;
            foreach (var cell in allCells)
            {
                if (cell.Affinity == AffinityType.Ice)
                    iceCount++;
            }

            var skillInstance = new Units.Skills.Snowball();
            skillInstance.SetIceCellCount(iceCount);
            unit.AddSkill(skillInstance);
        }
    }
}