using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class Fireball : Cell
    {
        public Fireball()
        {
            Affinity = AffinityType.Fire;
        }

        public override string Description()
        {
            return "发射一个火球，造成火焰伤害。";
        }

        public override string Name()
        {
            return "火球";
        }

        public override void PostApply(Unit unit, IReadOnlyList<Cell> allCells)
        {
            int fireCount = 0;
            foreach (Model.Tetri.Cell cell in allCells)
            {
                if (cell.Affinity == AffinityType.Fire)
                    fireCount++;
            }

            var skillInstance = new Units.Skills.Fireball();
            skillInstance.SetFireCellCount(fireCount);
            unit.AddSkill(skillInstance);
        }
    }
}