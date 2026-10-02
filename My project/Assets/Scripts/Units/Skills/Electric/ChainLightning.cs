using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ChainLightning : ActiveSkill
    {
        public ChainLightningLevelConfig Config { get; }

        private ChainLightningSkillConfig SkillConfig => (ChainLightningSkillConfig)Definition.Config;
        private Unit initialTarget;

        public ChainLightning(ChainLightningLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public override string Description()
        {
            return $"对最近的敌人发射一道闪电，造成 {Config.BaseDamage} 点伤害，" +
                   $"每次弹射伤害增加 {Config.DamageIncreasePercentage}%，最多弹射 {Config.MaxBounces} 次。";
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
            {
                return false;
            }

            initialTarget = Owner.FindClosestEnemyInRange(Config.Range);
            return initialTarget != null;
        }

        public override bool CanExecuteNow() => initialTarget != null && initialTarget.IsActive;

        protected override bool ExecuteCore()
        {
            if (initialTarget == null || !initialTarget.IsActive)
            {
                initialTarget = null;
                return false;
            }

            var projectile = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            projectile.Initialize(
                caster: Owner.SelfUnit,
                initialTarget: initialTarget,
                baseDamage: Config.BaseDamage,
                damageIncreasePercentage: Config.DamageIncreasePercentage,
                maxTargets: Config.MaxBounces + 1,
                chainRange: Config.Range,
                sourceSkill: this
            );
            projectile.Activate();
            initialTarget = null;
            return true;
        }

        public override string Name()
        {
            return "闪电链";
        }
    }
}