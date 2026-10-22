using System.Linq;
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

        private (StatValue LifeCostPercent, StatValue ShieldMultiplierPercent, StatValue Duration) CalcStats(int resonantCellCount) =>
            (
                new StatValue("生命值消耗比例(%)", Config.LifeCostPercent),
                new StatValue("原始生命消耗转护盾倍率(%)", SkillConfig.BaseShieldMultiplierPercent,
                    resonantCellCount * SkillConfig.ShieldMultiplierPercentPerResonantCell),
                new StatValue("护盾持续时间", SkillConfig.BuffDuration)
            );

        private int GetResonantCellCount() => Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;

        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            // 找到友方目标（不包括自己）
                cachedTarget = Owner.FindRandomAlly(
                range: float.MaxValue,
                includeSelf: false
            );
            if (cachedTarget == null)
                return false;

            return true;
        }

        protected override void ExecuteCore()
        {
            if (cachedTarget == null)
            return;

            var stats = CalcStats(GetResonantCellCount());
            float lifeCost = Owner.Attributes.CurrentHealth * (stats.LifeCostPercent.Final / 100f);
            float shieldValue = lifeCost * (stats.ShieldMultiplierPercent.Final / 100f);

            var buff = new Units.Buffs.LifeShieldBuff(
                SkillConfig.LifeShieldBuffDefinition,
                shieldValue,
                stats.Duration.Final,
                new Units.Buffs.BuffSource(Owner.SelfUnit, this)
            );
            var projectile = Object.Instantiate(
                SkillConfig.BuffProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            projectile.Init(Owner.SelfUnit, cachedTarget, buff);
            projectile.Activate();

            var damage = new Damages.Damage(lifeCost, Damages.DamageType.Skill);
            damage.SetSourceUnit(Owner.SelfUnit);
            damage.SetTargetUnit(Owner.SelfUnit);
            damage.SetSourceLabel(Name());
            Owner.SelfUnit.TakeDamage(damage);

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