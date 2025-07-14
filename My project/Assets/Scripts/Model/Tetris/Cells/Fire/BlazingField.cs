using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class BlazingField : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.BlazingField;
        public override AffinityType Affinity => AffinityType.Fire;

        public override string Description()
        {
            return "在目标区域生成焰域，对范围内敌人造成持续火焰伤害。";
        }

        public override string Name()
        {
            return "焰域";
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfigGroup as Units.Skills.BlazingFieldConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.BlazingField(config);
            unit.AddSkill(skillInstance);
        }
    }
}