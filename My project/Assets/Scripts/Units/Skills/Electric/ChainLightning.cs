using Model.Resonance;
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
            return $"对最近的敌人发射一道闪电，{CalcInitialDamage()}，" +
                   $"每次弹射伤害增加 {Config.DamageIncreasePercentage}%，最多弹射 {Config.MaxBounces} 次。";
        }

        protected override bool PrepareCastCore()
        {
            initialTarget = null;
            initialTarget = Owner.FindClosestEnemyInRange(Config.Range);
            return initialTarget != null;
        }

        protected override void ExecuteCore()
        {
            Projectiles.ChainLightning projectile = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            StatValue initialDamage = CalcInitialDamage();
            float energyDisruptionAmount = CalcEnergyDisruptionAmount();
            projectile.Initialize(
                caster: Owner.SelfUnit,
                initialTarget: initialTarget,
                baseDamage: initialDamage.Final,
                damageIncreasePercentage: Config.DamageIncreasePercentage,
                maxTargets: Config.MaxBounces + 1,
                chainRange: Config.Range,
                energyDisruptionAmount: energyDisruptionAmount,
                sourceSkill: this
            );
            projectile.Activate();
            initialTarget = null;
        }

        private StatValue CalcInitialDamage()
        {
            int electricCellCount = GetElectricCellCount();

            return new StatValue(
                "首跳伤害",
                Config.BaseDamage,
                electricCellCount * Config.DamagePerResonantCell
            );
        }

        private float CalcEnergyDisruptionAmount()
        {
            return Config.BaseEnergyDisruptionAmount
                     + GetElectricCellCount() * Config.EnergyDisruptionAmountPerResonantCell;
        }

        private int GetElectricCellCount()
        {
            if (Owner == null)
            {
                return 0;
            }

            return Owner.GetResonanceCount(Definition.Resonance);
        }

        public override string Name()
        {
            return "闪电链";
        }
    }
}