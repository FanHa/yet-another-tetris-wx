using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Model.Tetri
{
    public class AttackBoost : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.AttackBoost;
        public override AffinityType Affinity => AffinityType.Wind;

        public override string Description()
        {
            return "短时间内提升攻击速度。每个风系方块增加额外的攻击速度。";
        }

        public override string Name()
        {
            return "攻击加速";
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.AttackBoostConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.AttackBoost(config);
            unit.AddSkill(skillInstance);
        }
    }
}