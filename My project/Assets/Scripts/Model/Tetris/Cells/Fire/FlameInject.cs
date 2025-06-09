using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class FlameInject : Cell
    {
        public float BaseFireDamage = 1f;
        public float FireDamagePerCell = 1f;
        public float BaseDotDps = 2f;
        public float DotDpsPerCell = 0.5f;
        public float BaseDotDuration = 2f;
        public float DotDurationPerCell = 1f;

        public FlameInject()
        {
            Affinity = AffinityType.Fire;
        }

        public override string Description()
        {
            return "攻击时对目标附加火焰伤害并施加灼烧。";
        }

        public override string Name()
        {
            return "炎附";
        }

        public override void PostApply(Unit unit, IReadOnlyList<Cell> allCells)
        {
            int fireCellCount = 0;
            foreach (var cell in allCells)
            {
                if (cell.Affinity == AffinityType.Fire)
                    fireCellCount++;
            }

            var skillInstance = new Units.Skills.FlameInject();
            skillInstance.SetFireCellCount(fireCellCount);
            unit.AddSkill(skillInstance);
        }
    }
}