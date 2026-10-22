using System.Linq;
using Model.Buffs;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowStep : ActiveSkill
    {
        public ShadowStepLevelConfig Config { get; }

        public ShadowStep(ShadowStepLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
            InitializeInitialEnergy(config.InitEnergy);
        }

        private struct ShadowStepStats
        {
            public StatValue VulnerabilityPercent;
            public StatValue Damage;
            public StatValue DebuffDuration;
        }

        private ShadowStepStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();
            var vulnerabilityStats = GetVulnerabilityDefinition().CalculateStats(resonantCellCount);
            return new ShadowStepStats
            {
                VulnerabilityPercent = vulnerabilityStats.ExtraDamagePercent,
                Damage = new StatValue(
                    "技能伤害",
                    Config.Damage,
                    resonantCellCount * Config.DamagePerResonantCell
                ),
                DebuffDuration = vulnerabilityStats.Duration
            };
        }

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }

        private Model.Buffs.VulnerabilityBuffDefinition GetVulnerabilityDefinition()
        {
            return (Model.Buffs.VulnerabilityBuffDefinition)((ShadowStepSkillConfig)Definition.Config).VulnerabilityBuffDefinition;
        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            Unit targetEnemy = Owner.GetActiveEnemies()
                .OrderBy(enemy => enemy.Attributes.MaxHealth.finalValue)
                .FirstOrDefault();
            if (targetEnemy == null)
                return;

            Vector3 dir = (targetEnemy.transform.position - Owner.transform.position).normalized;
            Vector3 targetPos = targetEnemy.transform.position + dir * 1.2f; // 1.2f为身后距离，可调整

            Owner.Teleport(targetPos);

            var vulnerabilityBuff = new Units.Buffs.Vulnerability(
                GetVulnerabilityDefinition(),
                extraDamagePercent: stats.VulnerabilityPercent.Final,
                buffDuration: BuffDuration.ForSeconds(stats.DebuffDuration.Final),
                source: new Units.Buffs.BuffSource(Owner.SelfUnit, this)
            );
            targetEnemy.AddBuff(vulnerabilityBuff);

            // 造成伤害
            var damage = new Damages.Damage(Config.Damage, Damages.DamageType.Hit);
            damage.SetSourceUnit(Owner.SelfUnit);
            damage.SetTargetUnit(targetEnemy);
            damage.SetSourceLabel(Name());
            targetEnemy.TakeDamage(damage);
            // todo 动画

        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.VulnerabilityPercent}\n" +
                $"{stats.Damage}\n" +
                $"{stats.DebuffDuration}\n";
        }

            public static string DescriptionStatic() => "闪现到最大生命值最低的敌人身后,施加易伤并造成伤害";
            public override string Name() => NameStatic();
        public static string NameStatic() => "影步";
    }
}