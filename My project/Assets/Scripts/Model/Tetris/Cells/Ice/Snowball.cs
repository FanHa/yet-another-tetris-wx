using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class Snowball : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.Snowball;
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

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.SnowballConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.Snowball(config);
            unit.AddSkill(skillInstance);
        }
    }
}