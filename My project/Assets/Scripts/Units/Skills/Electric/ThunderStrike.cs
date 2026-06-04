using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class ThunderStrike : ActiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.ThunderStrike;

        public ThunderStrikeConfig Config { get; }
        private Unit cachedTarget;

        public ThunderStrike(ThunderStrikeConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public override bool IsReady()
        {
            if (!base.IsReady())
                return false;

            if (!Owner.TryGetClosestEnemyInAttackRange(out var found))
                return false;

            cachedTarget = found;
            return true;
        }

        public override bool CanExecuteNow() =>
            cachedTarget != null && cachedTarget.IsActive;

        protected override bool ExecuteCore()
        {
            var stats = CalcStats();

            var prefab = Owner.ProjectileConfig.ThunderStrikePrefab;
            if (prefab == null)
                return false;

            GameObject effectInstance = Object.Instantiate(
                prefab,
                cachedTarget.transform.position,
                Quaternion.identity
            );

            var thunderStrikeEffect = effectInstance.GetComponent<Units.Projectiles.ThunderStrikeProjectile>();
            if (thunderStrikeEffect == null)
            {
                Object.Destroy(effectInstance);
                return false;
            }

            thunderStrikeEffect.Init(
                Owner,
                cachedTarget,
                stats.Damage.Final,
                stats.StunDuration,
                this
            );
            thunderStrikeEffect.Activate();

            cachedTarget = null;
            return true;
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + "\n" +
                $"{stats.Damage}\n" +
                $"眩晕持续时间: <final>{stats.StunDuration}</final>";
        }

        public static string DescriptionStatic() => "对攻击范围内一个敌人头顶降下雷击，造成伤害并施加眩晕";

        public override string Name() => NameStatic();
        public static string NameStatic() => "雷击";

        private struct ThunderStrikeStats
        {
            public StatValue Damage;
            public float StunDuration;
        }

        private ThunderStrikeStats CalcStats()
        {
            int electricCellCount = GetElectricCellCount();

            float rawStunDuration = Config.BaseStunDuration + electricCellCount * Config.StunDurationPerElectricCell;
            float stunDuration = Mathf.Min(rawStunDuration, Config.MaxStunDuration);

            return new ThunderStrikeStats
            {
                Damage = new StatValue("技能伤害", Config.BaseDamage, electricCellCount * Config.DamagePerElectricCell),
                StunDuration = stunDuration
            };
        }

        private int GetElectricCellCount()
        {
            return Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Electric, out var count) ? count : 0;
        }
    }
}
