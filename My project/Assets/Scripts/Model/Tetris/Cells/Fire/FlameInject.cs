using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class FlameInject : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameInject;
        public override AffinityType Affinity => AffinityType.Fire;

        public override string Description()
        {
            return Units.Skills.FlameInject.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.FlameInject.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.FlameInjectSkillConfig;
            var config = configGroup?.GetLevelConfig(Level);
            var skillInstance = new Units.Skills.FlameInject(config);
            unit.AddSkill(skillInstance);
        }
    }
}