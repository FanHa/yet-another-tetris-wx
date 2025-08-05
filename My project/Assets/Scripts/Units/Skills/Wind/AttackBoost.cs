using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class AttackBoost : ActiveSkill
    {
        public AttackBoostConfig Config { get; }

        public AttackBoost(AttackBoostConfig config)
        {
            Config = config;
            this.RequiredEnergy = config.RequiredEnergy;
        }

        public override CellTypeId CellTypeId => CellTypeId.AttackBoost;

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

        protected override bool ExecuteCore()
        {
            var stats = CalcStats();
            var buff = new Buffs.AttackBoostBuff(
                duration: stats.Duration.Final,
                atkSpeedPercent: stats.AtkSpeedPercent.Final,
                sourceUnit: Owner,
                sourceSkill: this
            );

            var prefab = Owner.ProjectileConfig.BuffProjectilePrefab;
            var projectileObj = Object.Instantiate(prefab, Owner.transform.position, Quaternion.identity);
            var projectile = projectileObj.GetComponent<Units.Projectiles.BuffProjectile>();
            projectile.Init(Owner, Owner, buff); // 目标为自己
            projectile.Activate();
            return true;
        }
    }
}