using Model.Tetri;
using Model.Skills;

namespace Units.Skills
{
    public class LifeEcho : Skill
    {
        public LifeEchoLevelConfig Config { get; }

        public LifeEcho(LifeEchoLevelConfig config)
        {
            Config = config;
        }

        public override string Name() => NameStatic();
        public static string NameStatic() => "生命回响";

        private int GetResonantCellCount() => Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;

        public override string Description()
        {
            return DescriptionStatic() + ":\n" +
                ((LifeEchoSkillConfig)Definition.Config).LifeEchoBuffDefinition.DescribeEffect(GetResonantCellCount());
        }
        public static string DescriptionStatic() => "反弹受到的伤害";

        public override void OnPassiveEffectsApplication()
        {
            Owner.AddBuff(new Units.Buffs.LifeEchoBuff(
                ((LifeEchoSkillConfig)Definition.Config).LifeEchoBuffDefinition,
                GetResonantCellCount(),
                new Units.Buffs.BuffSource(Owner.SelfUnit, this)
            ));
        }
    }
}