using Model.Tetri;
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

        private struct AttackBoostStats
        {
            public StatValue AtkSpeedPercent;
            public StatValue Duration;
        }

        private AttackBoostStats CalcStats()
        {
            int windCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Wind, out var count) ? count : 0;
            return new AttackBoostStats
            {
                AtkSpeedPercent = new StatValue("攻击速度提升(%)", Config.AtkSpeedPercent, windCellCount * Config.AtkSpeedAdditionPercentPerWindCell),
                Duration = new StatValue("持续时间", Config.Duration)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Duration}\n" +
                $"{stats.AtkSpeedPercent}";
        }

        public static string DescriptionStatic() => "提升攻击速度";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "攻击加速";

        protected override void ExecuteCore()
        {
            var stats = CalcStats();
            var buff = new Buffs.AttackBoostBuff(
                ((AttackBoostSkillConfig)Definition.Config).AttackBoostBuffDefinition,
                duration: stats.Duration.Final,
                atkSpeedPercent: stats.AtkSpeedPercent.Final,
                sourceUnit: Owner.SelfUnit,
                sourceSkill: this
            );

            var projectile = Object.Instantiate(
                SkillConfig.BuffProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            projectile.Init(Owner.SelfUnit, Owner.SelfUnit, buff); // 目标为自己
            projectile.Activate();
        }
    }
}