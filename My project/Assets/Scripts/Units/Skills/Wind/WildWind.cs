using Model;
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
            public StatBreakdownDisplay Damage;
            public StatBreakdownDisplay Radius;
            public StatBreakdownDisplay Duration;
        }

        private WildWindStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();
            return new WildWindStats
            {
                Damage = new StatBreakdownDisplay(StatLabel.Damage, Config.Damage, resonantCellCount * Config.DamageAdditionPerResonantCell),
                Radius = new StatBreakdownDisplay(StatLabel.ActionRadius, Config.Radius, resonantCellCount * Config.RadiusPerResonantCell),
                Duration = new StatBreakdownDisplay(StatLabel.WindFieldDuration, Config.Duration, resonantCellCount * Config.DurationPerResonantCell)
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
                Context.Transform.position,
                Quaternion.identity
            );
            wildWind.Initialize(
                caster: Context.Unit,
                radius: stats.Radius.FinalValue,
                duration: stats.Duration.FinalValue,
                damage: stats.Damage.FinalValue,
                debuffDefinition: SkillConfig.WildWindDebuffDefinition,
                resonantCellCount: GetResonantCellCount(),
                sourceSkill: this // 传递技能引用
            );
            wildWind.Activate();
        }
    private int GetResonantCellCount() => Context != null ? Context.GetResonanceCount(Definition.Resonance) : 0;

        
    }
}