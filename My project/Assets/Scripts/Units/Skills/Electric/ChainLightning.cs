using Model;
using Presentation;
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

        public override string DescribeEffect()
        {
            return $"对最近的敌人发射一道闪电，{CalcInitialDamage()}，" +
                   $"每次弹射伤害增加 {Config.DamageIncreasePercentage}%，最多弹射 {Config.MaxBounces} 次。";
        }

        protected override bool PrepareCastCore()
        {
            initialTarget = null;
            initialTarget = Context.FindClosestEnemyInRange(Config.Range);
            return initialTarget != null;
        }

        protected override void ExecuteCore()
        {
            Projectiles.ChainLightning projectile = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Context.Transform.position,
                Quaternion.identity
            );
            StatBreakdownDisplay initialDamage = CalcInitialDamage();
            float energyDisruptionAmount = CalcEnergyDisruptionAmount();
            projectile.Initialize(
                caster: Context.Unit,
                initialTarget: initialTarget,
                baseDamage: initialDamage.FinalValue,
                damageIncreasePercentage: Config.DamageIncreasePercentage,
                maxTargets: Config.MaxBounces + 1,
                chainRange: Config.Range,
                energyDisruptionAmount: energyDisruptionAmount,
                sourceSkill: this
            );
            projectile.Activate();
            initialTarget = null;
        }

        private StatBreakdownDisplay CalcInitialDamage()
        {
            int electricCellCount = Context.GetResonanceCount(Definition.Resonance);

            return new StatBreakdownDisplay(
                StatLabel.FirstChainDamage,
                Config.BaseDamage,
                electricCellCount * Config.DamagePerResonantCell
            );
        }

        private float CalcEnergyDisruptionAmount()
        {
            return Config.BaseEnergyDisruptionAmount
                     + Context.GetResonanceCount(Definition.Resonance) * Config.EnergyDisruptionAmountPerResonantCell;
        }

        public override string Name()
        {
            return "闪电链";
        }
    }
}