using System.Linq;
using Model;
using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class LifeShield : ActiveSkill
    {
        public LifeShieldLevelConfig Config { get; }
        private Unit cachedTarget;

        private LifeShieldSkillConfig SkillConfig => (LifeShieldSkillConfig)Definition.Config;

        public LifeShield(LifeShieldLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        private (StatBreakdownDisplay LifeCostPercent, StatBreakdownDisplay ShieldMultiplierPercent, string Duration) CalcStats(int resonantCellCount) =>
            (
                new StatBreakdownDisplay(StatLabel.LifeCostPercent, Config.LifeCostPercent),
                new StatBreakdownDisplay(StatLabel.ShieldMultiplierPercent, SkillConfig.BaseShieldMultiplierPercent,
                    resonantCellCount * SkillConfig.ShieldMultiplierPercentPerResonantCell),
                SkillConfig.LifeShieldBuffDefinition.BaseDuration.ToDisplayString("护盾持续时间")
            );

        private int GetResonantCellCount() => Context != null ? Context.GetResonanceCount(Definition.Resonance) : 0;

        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            // 找到友方目标（不包括自己）
            cachedTarget = Context.FindRandomAlly();
            if (cachedTarget == null)
                return false;

            return true;
        }

        protected override void ExecuteCore()
        {
            if (cachedTarget == null)
            return;

            var stats = CalcStats(GetResonantCellCount());
            float lifeCost = Context.Attributes.CurrentHealth * (stats.LifeCostPercent.FinalValue / 100f);
            float shieldValue = lifeCost * (stats.ShieldMultiplierPercent.FinalValue / 100f);

            var buff = new Units.Buffs.LifeShieldBuff(
                SkillConfig.LifeShieldBuffDefinition,
                shieldValue,
                SkillConfig.LifeShieldBuffDefinition.BaseDuration,
                new Units.Buffs.BuffSource(Context.Unit, this)
            );
            var projectile = Object.Instantiate(
                SkillConfig.BuffProjectilePrefab,
                Context.Transform.position,
                Quaternion.identity
            );
            projectile.Init(Context.Unit, cachedTarget, buff);
            projectile.Activate();

            var damage = new Damages.Damage(lifeCost, Damages.DamageType.Skill);
            damage.SetSourceUnit(Context.Unit);
            damage.SetTargetUnit(Context.Unit);
            damage.SetSourceLabel(Name());
            Context.Unit.TakeDamage(damage);

            cachedTarget = null;
        }

        public override string Description()
        {
            var stats = CalcStats(GetResonantCellCount());
            return
                DescriptionStatic() + "\n" +
                $"{stats.LifeCostPercent}\n{stats.ShieldMultiplierPercent}\n{stats.Duration}";
        }

        public static string DescriptionStatic() => "消耗自身血量,为一个友方单位添加生命护盾，抵挡伤害。";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "生命护盾";
    }
}