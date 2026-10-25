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

        public override string DescribeEffect()
        {
            return DescriptionStatic() + ":\n" +
                ((LifeEchoSkillConfig)Definition.Config).LifeEchoBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }
        public static string DescriptionStatic() => "反弹受到的伤害";

        public override void OnPassiveEffectsApplication()
        {
            Context.AddBuff(new Units.Buffs.LifeEchoBuff(
                ((LifeEchoSkillConfig)Definition.Config).LifeEchoBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Units.Buffs.BuffSource(Context.Unit, this)
            ));
        }
    }
}