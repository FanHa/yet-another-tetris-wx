using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class ShadowStep : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.ShadowStep;
        public ShadowStepConfig Config { get; }

        public ShadowStep(ShadowStepConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
            CurrentEnergy = config.InitEnergy;
        }

        private struct ShadowStepStats
        {
            public StatValue VulnerabilityPercent;
            public StatValue Damage;
            public StatValue DebuffDuration;
        }

        private ShadowStepStats CalcStats()
        {
            int shadowCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Shadow, out var count) ? count : 0;
            return new ShadowStepStats
            {
                VulnerabilityPercent = new StatValue(
                    "易伤百分比",
                    Config.VulnerabilityPercent,
                    shadowCellCount * Config.VulnerabilityPercentPerShadowCell
                ),
                Damage = new StatValue(
                    "技能伤害",
                    Config.Damage,
                    shadowCellCount * Config.DamagePerShadowCell
                ),
                DebuffDuration = new StatValue(
                    "易伤持续时间",
                    Config.DebuffDuration
                )
            };
        }

        protected override bool ExecuteCore()
        {
            var stats = CalcStats();

            Unit targetEnemy = Owner.UnitManager.FindWeakestEnemy(Owner);
            if (targetEnemy == null)
                return false;

            Vector3 dir = (targetEnemy.transform.position - Owner.transform.position).normalized;
            Vector3 targetPos = targetEnemy.transform.position + dir * 1.2f; // 1.2f为身后距离，可调整

            Owner.transform.position = targetPos;

            var vulnerabilityBuff = new Units.Buffs.Vulnerability(
                buffDuration: stats.DebuffDuration.Final, // 可根据需求调整持续时间
                extraDamagePercent: stats.VulnerabilityPercent.Final,
                sourceUnit: Owner,
                sourceSkill: this
            );
            targetEnemy.AddBuff(vulnerabilityBuff);

            // 造成伤害
            var damage = new Damages.Damage(Config.Damage, Damages.DamageType.Hit);
            damage.SetSourceUnit(Owner);
            damage.SetTargetUnit(targetEnemy);
            damage.SetSourceLabel(Name());
            targetEnemy.TakeDamage(damage);
            // todo 动画

            return true;
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

        public static string DescriptionStatic() => "闪现到最脆弱敌人身后,施加易伤并造成伤害";

        public override string Name() => NameStatic();
        public static string NameStatic() => "影步";
    }
}