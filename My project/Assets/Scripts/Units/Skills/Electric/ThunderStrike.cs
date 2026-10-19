using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class ThunderStrike : ActiveSkill
    {
        public ThunderStrikeLevelConfig Config { get; }
        private Unit cachedTarget;

        private ThunderStrikeSkillConfig SkillConfig => (ThunderStrikeSkillConfig)Definition.Config;

        public ThunderStrike(ThunderStrikeLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        protected override bool PrepareCastCore()
        {
            cachedTarget = null;
            if (!Owner.TryGetClosestEnemyInAttackRange(out var found))
                return false;

            cachedTarget = found;
            return true;
        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            Units.Projectiles.ThunderStrikeProjectile thunderStrikeEffect = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                cachedTarget.transform.position,
                Quaternion.identity
            );

            thunderStrikeEffect.Init(
                Owner.SelfUnit,
                cachedTarget,
                stats.Damage.Final,
                stats.EnergyDisruptionAmount,
                this
            );
            thunderStrikeEffect.Activate();

            cachedTarget = null;
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + "\n" +
                $"{stats.Damage}";
        }

        public static string DescriptionStatic() => "对攻击范围内一个敌人头顶降下雷击，造成伤害并削减其随机技能蓄能";

        public override string Name() => NameStatic();
        public static string NameStatic() => "雷击";

        private struct ThunderStrikeStats
        {
            public StatValue Damage;
            public float EnergyDisruptionAmount;
        }

        private ThunderStrikeStats CalcStats()
        {
            int electricCellCount = GetElectricCellCount();
            float energyDisruptionAmount =
                Config.BaseEnergyDisruptionAmount + electricCellCount * Config.EnergyDisruptionAmountPerElectricCell;

            return new ThunderStrikeStats
            {
                Damage = new StatValue("技能伤害", Config.BaseDamage, electricCellCount * Config.DamagePerElectricCell),
                EnergyDisruptionAmount = energyDisruptionAmount
            };
        }

        private int GetElectricCellCount()
        {
            return Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Electric, out var count) ? count : 0;
        }
    }
}
