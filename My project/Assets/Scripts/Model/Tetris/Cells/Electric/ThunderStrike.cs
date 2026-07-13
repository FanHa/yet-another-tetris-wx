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
            var configGroup = SkillConfig as Units.Skills.ThunderStrikeSkillConfig;
            var config = configGroup?.TryGetLevelConfig(Level, out var levelConfig) == true ? levelConfig : null;
            var skillInstance = new Units.Skills.ThunderStrike(config);
            unit.AddSkill(skillInstance);
        }
    }
}
