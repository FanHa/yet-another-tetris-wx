using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class FlameRing : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.FlameRing;
        public override AffinityType Affinity => AffinityType.Fire;

        public override string Description()
        {
            return Units.Skills.FlameRing.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.FlameRing.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = SkillConfig as Units.Skills.FlameRingSkillConfig;
            var config = configGroup?.GetLevelConfig(Level);
            var skillInstance = new Units.Skills.FlameRing(config);
            unit.AddSkill(skillInstance);
        }
    }
}