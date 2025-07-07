using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class IceShield : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.IceShield;
        public IceShield()
        {
            Affinity = AffinityType.Ice;
        }

        public override string Description() => "获得一次性冰霜护盾技能，被攻击时反制攻击者。";
        public override string Name() => "冰霜护盾";

        public override void PostApply(Unit unit, IReadOnlyList<Cell> allCells)
        {
            int iceCellCount = 0;
            foreach (var cell in allCells)
            {
                if (cell.Affinity == AffinityType.Ice)
                    iceCellCount++;
            }

            var configGroup = SkillConfigGroup as Units.Skills.IceShieldConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.IceShield(config);
            skillInstance.SetIceCellCount(iceCellCount);
            unit.AddSkill(skillInstance);
        }
        
    }
}