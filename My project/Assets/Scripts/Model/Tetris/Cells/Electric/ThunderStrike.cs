using Units;

namespace Model.Tetri
{
    public class ThunderStrike : Cell
    {
        public override CellTypeId CellTypeId => CellTypeId.ThunderStrike;
        public override AffinityType Affinity => AffinityType.Electric;

        public override string Description()
        {
            return Units.Skills.ThunderStrike.DescriptionStatic();
        }

        public override string Name()
        {
            return Units.Skills.ThunderStrike.NameStatic();
        }

        public override void Apply(Unit unit)
        {
            var configGroup = Config as Units.Skills.ThunderStrikeSkillConfig;
            var config = configGroup?.GetLevelConfig(Level);
            var skillInstance = new Units.Skills.ThunderStrike(config);
            unit.AddSkill(skillInstance);
        }
    }
}

