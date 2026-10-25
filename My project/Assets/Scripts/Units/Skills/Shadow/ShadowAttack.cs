using Model.Skills;

namespace Units.Skills
{
    public class ShadowAttack : Skill
    {
        public ShadowAttackLevelConfig Config { get; }

        private ShadowAttackSkillConfig SkillConfig => (ShadowAttackSkillConfig)Definition.Config;

        public ShadowAttack(ShadowAttackLevelConfig config)
        {
            Config = config;
        }

        public override string DescribeEffect()
        {
            return
                DescriptionStatic() + ":\n" +
                SkillConfig.ShadowAttackBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance)) + "\n";
        }
        public static string DescriptionStatic() => "攻击时附加易伤";

        public override string Name() => NameStatic();
        public static string NameStatic() => "影袭";

        public override void OnPassiveEffectsApplication()
        {
            var buff = new Buffs.ShadowAttackBuff(
                SkillConfig.ShadowAttackBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            );
            Context.AddBuff(buff);
        }
    }
}