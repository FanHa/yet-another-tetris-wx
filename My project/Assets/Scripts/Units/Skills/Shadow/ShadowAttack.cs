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

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }

        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                SkillConfig.ShadowAttackBuffDefinition.DescribeEffect(GetResonantCellCount()) + "\n";
        }
        public static string DescriptionStatic() => "攻击时附加易伤";

        public override string Name() => NameStatic();
        public static string NameStatic() => "影袭";

        public override void OnApplyPassive()
        {
            var buff = new Buffs.ShadowAttackBuff(
                SkillConfig.ShadowAttackBuffDefinition,
                GetResonantCellCount(),
                new Buffs.BuffSource(Owner.SelfUnit, this)
            );
            Owner.AddBuff(buff);
        }
    }
}