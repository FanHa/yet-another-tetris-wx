using System.Collections.Generic;
using Units;

namespace Model.Tetri
{
    public class Fireball : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.Fireball;
        public override AffinityType Affinity => AffinityType.Fire;

        public override string Description()
        {
            return Units.Skills.Fireball.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.Fireball.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = Config as Units.Skills.FireballSkillConfig;
            var config = configGroup?.GetLevelConfig(Level);
            var skillInstance = new Units.Skills.Fireball(config);
            unit.AddSkill(skillInstance);
        }
    }
}
