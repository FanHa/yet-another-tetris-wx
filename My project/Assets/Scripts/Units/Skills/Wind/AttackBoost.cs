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

        public override string Description()
        {
            return DescriptionStatic();
        }
        public static string DescriptionStatic() => "短时间内攻击速度提升";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "攻击加速";

        protected override bool ExecuteCore(Unit caster)
        {
            int windCellCount = caster.CellCounts.TryGetValue(AffinityType.Wind, out var count) ? count : 0;
            float atkSpeedPercent = Config.AtkSpeedPercent + windCellCount * Config.AtkSpeedAdditionPercentPerWindCell;
            var buff = new Buffs.AttackBoostBuff(
                duration: Config.Duration,
                atkSpeedPercent: atkSpeedPercent,
                sourceUnit: caster,
                sourceSkill: this
            );

            var prefab = caster.ProjectileConfig.BuffProjectilePrefab;
            var projectileObj = Object.Instantiate(prefab, caster.transform.position, Quaternion.identity);
            var projectile = projectileObj.GetComponent<Units.Projectiles.BuffProjectile>();
            projectile.Init(caster, caster, buff); // 目标为自己
            projectile.Activate();
            return true;
        }
    }
}