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

        private int GetResonantCellCount()
        {
            return Context != null ? Context.GetResonanceCount(Definition.Resonance) : 0;
        }

        public override string Description()
        {
            return
                DescriptionStatic() + ":\n" +
                SkillConfig.AttackBoostBuffDefinition.DescribeEffect(GetResonantCellCount());
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
                GetResonantCellCount(),
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