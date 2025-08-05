using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class WildWind : ActiveSkill
    {
        public WildWindConfig Config { get; }

        public WildWind(WildWindConfig config)
        {
            this.Config = config;
            this.RequiredEnergy = config.RequiredEnergy;
        }

        public override CellTypeId CellTypeId => CellTypeId.WildWind;

        private struct WildWindStats
        {
            public StatValue Damage;
            public StatValue Radius;
            public StatValue Duration;
            public StatValue DebuffDuration;
            public StatValue MoveSlowPercent;
            public StatValue AtkReducePercent;
        }

        private WildWindStats CalcStats()
        {
            int windCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Wind, out var count) ? count : 0;
            return new WildWindStats
            {
                Damage = new StatValue("伤害", Config.Damage, windCellCount * Config.DamageAdditionPerWindCell),
                Radius = new StatValue("作用半径", Config.Radius),
                Duration = new StatValue("风场持续时间", Config.Duration),
                DebuffDuration = new StatValue("DeBuff持续时间", Config.DebuffDuration),
                MoveSlowPercent = new StatValue("DeBuff移动速度降低(%)", Config.MoveSlowPercent),
                AtkReducePercent = new StatValue("DeBuff攻击力降低(%)", Config.AtkReducePercent)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + $"\n" +
                $"{stats.Damage}\n" +
                $"{stats.Radius}\n" +
                $"{stats.Duration}\n" +
                $"{stats.DebuffDuration}\n" +
                $"{stats.MoveSlowPercent}\n" +
                $"{stats.AtkReducePercent}";
        }

        public static string DescriptionStatic() => "在自身周围制造一阵狂风, 击退敌人并造成伤害和减速Debuff.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "狂风";

        protected override bool ExecuteCore()
        {
            var stats = CalcStats();

            var prefab = Owner.ProjectileConfig.WildWindPrefab;
            var wildWindObj = Object.Instantiate(prefab, Owner.transform.position, Quaternion.identity);
            var wildWind = wildWindObj.GetComponent<Units.Projectiles.WildWind>();
            wildWind.Initialize(
                caster: Owner,
                radius: stats.Radius.Final,
                duration: stats.Duration.Final,
                damage: stats.Damage.Final,
                debuffDuration: stats.DebuffDuration.Final,
                moveSlowPercent: (int)stats.MoveSlowPercent.Final,
                atkReducePercent: (int)stats.AtkReducePercent.Final
            );
            wildWind.Activate();
            return true;
        }

        
    }
}