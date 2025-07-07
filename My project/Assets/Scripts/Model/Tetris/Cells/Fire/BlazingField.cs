using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class BlazingField : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.BlazingField;
        public BlazingField()
        {
            Affinity = AffinityType.Fire;
        }

        public override string Description()
        {
            return "在目标区域生成焰域，对范围内敌人造成持续火焰伤害。";
        }

        public override string Name()
        {
            return "焰域";
        }

        public override void PostApply(Unit unit, IReadOnlyList<Cell> allCells)
        {
            int fireCount = 0;
            foreach (var cell in allCells)
            {
                if (cell.Affinity == AffinityType.Fire)
                    fireCount++;
            }

            var configGroup = SkillConfigGroup as Units.Skills.BlazingFieldConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.BlazingField(config);
            skillInstance.SetFireCellCount(fireCount);
            unit.AddSkill(skillInstance);
        }
    }
}