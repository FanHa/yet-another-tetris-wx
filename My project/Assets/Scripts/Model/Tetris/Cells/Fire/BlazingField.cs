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
            return Units.Skills.BlazingField.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.BlazingField.NameStatic();
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