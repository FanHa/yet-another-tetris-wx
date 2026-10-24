using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class AttackBoost : ActiveSkill
    {
        public AttackBoostLevelConfig Config { get; }

        private AttackBoostSkillConfig SkillConfig => (AttackBoostSkillConfig)Definition.Config;

        public AttackBoost(AttackBoostLevelConfig config)
        {
            Config = config;
            this.RequiredEnergy = config.RequiredEnergy;
        }

        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                SkillConfig.AttackBoostBuffDefinition.DescribeEffect(Context.GetResonanceCount(Definition.Resonance));
        }

        public static string DescriptionStatic() => "提升攻击速度";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "攻击加速";

        protected override void ExecuteCore()
        {
            var buff = new Buffs.AttackBoostBuff(
                SkillConfig.AttackBoostBuffDefinition,
                Context.GetResonanceCount(Definition.Resonance),
                new Buffs.BuffSource(Context.Unit, this)
            );

            var projectile = Object.Instantiate(
                SkillConfig.BuffProjectilePrefab,
                Context.Transform.position,
                Quaternion.identity
            );
            projectile.Init(Context.Unit, Context.Unit, buff); // 目标为自己
            projectile.Activate();
        }
    }
}