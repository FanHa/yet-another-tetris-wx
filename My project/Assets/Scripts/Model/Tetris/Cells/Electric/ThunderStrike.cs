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
            var configGroup = SkillConfigGroup as Units.Skills.ThunderStrikeConfigGroup;
            var config = configGroup?.LevelConfigs[Level - 1];
            var skillInstance = new Units.Skills.ThunderStrike(config);
            unit.AddSkill(skillInstance);
        }
    }
}
