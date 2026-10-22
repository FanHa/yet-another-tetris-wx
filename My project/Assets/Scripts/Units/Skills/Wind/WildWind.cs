using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class WildWind : ActiveSkill
    {
        public WildWindLevelConfig Config { get; }

        private WildWindSkillConfig SkillConfig => (WildWindSkillConfig)Definition.Config;

        public WildWind(WildWindLevelConfig config)
        {
            this.Config = config;
            this.RequiredEnergy = config.RequiredEnergy;
        }

        private struct WildWindStats
        {
            public StatValue Damage;
            public StatValue Radius;
            public StatValue Duration;
        }

        private WildWindStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();
            return new WildWindStats
            {
                Damage = new StatValue("伤害", Config.Damage, resonantCellCount * Config.DamageAdditionPerResonantCell),
                Radius = new StatValue("作用半径", Config.Radius, resonantCellCount * Config.RadiusPerResonantCell),
                Duration = new StatValue("风场持续时间", Config.Duration, resonantCellCount * Config.DurationPerResonantCell)
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
                SkillConfig.WildWindDebuffDefinition.DescribeEffect(GetResonantCellCount());
        }

        public static string DescriptionStatic() => "在自身周围制造一阵狂风, 击退敌人并造成伤害和减速Debuff.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "狂风";

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            var wildWind = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                Owner.transform.position,
                Quaternion.identity
            );
            wildWind.Initialize(
                caster: Owner.SelfUnit,
                radius: stats.Radius.Final,
                duration: stats.Duration.Final,
                damage: stats.Damage.Final,
                debuffDefinition: SkillConfig.WildWindDebuffDefinition,
                resonantCellCount: GetResonantCellCount(),
                sourceSkill: this // 传递技能引用
            );
            wildWind.Activate();
        }
    private int GetResonantCellCount() => Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;

        
    }
}