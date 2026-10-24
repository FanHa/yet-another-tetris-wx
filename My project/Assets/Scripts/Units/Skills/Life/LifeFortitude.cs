using Model.Skills;

namespace Units.Skills
{
    public class LifeFortitude : Skill
    {
        public LifeFortitudeLevelConfig Config { get; }

        public LifeFortitude(LifeFortitudeLevelConfig config)
        {
            Config = config;
        }

        public override string Name() => NameStatic();
        public static string NameStatic() => "生命强化";

        private int GetResonantCellCount() => Context != null ? Context.GetResonanceCount(Definition.Resonance) : 0;

        public override string Description() =>
            DescriptionStatic() + "\n" +
            ((LifeFortitudeSkillConfig)Definition.Config).BuffDefinition.DescribeEffect(
                Config.MaxHealthIncrease,
                GetResonantCellCount()
            );

        public static string DescriptionStatic() => "被动：提升自身最大生命值";

        public override void OnPassiveEffectsApplication()
        {
            var buffDefinition = ((LifeFortitudeSkillConfig)Definition.Config).BuffDefinition;
            Context.AddBuff(new Buffs.LifeFortitudeBuff(
                buffDefinition,
                Config.MaxHealthIncrease,
                GetResonantCellCount(),
                new Buffs.BuffSource(Context.Unit, this)
            ));
        }
    }
}