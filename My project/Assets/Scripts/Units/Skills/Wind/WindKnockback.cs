using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class WindKnockback : Skill
    {
        public WindKnockbackLevelConfig Config { get; }

        public WindKnockback(WindKnockbackLevelConfig config)
        {
            Config = config;
        }

        private struct WindKnockbackStats
        {
            public StatValue KnockbackDistance;
            public StatValue MaxKnockbackDistance;
        }

        private WindKnockbackStats CalcStats()
        {
            int resonantCellCount = Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
            return new WindKnockbackStats
            {
                KnockbackDistance = new StatValue("击退距离", Config.BaseKnockbackDistance, resonantCellCount * Config.KnockbackDistancePerResonantCell),
                MaxKnockbackDistance = new StatValue("最大击退距离", Config.MaxKnockbackDistance)
            };
        }

        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.KnockbackDistance}\n" +
                $"{stats.MaxKnockbackDistance}";
        }

        public static string DescriptionStatic() => "普通攻击命中时对目标造成小幅击退";

        public override string Name() => NameStatic();

        public static string NameStatic() => "轻风击退";

        public override void OnApplyPassive()
        {
            var stats = CalcStats();
            float dist = Mathf.Min(stats.KnockbackDistance.Final, stats.MaxKnockbackDistance.Final);
            Owner.AddBuff(Buffs.BuffFactory.Create(
                ((WindKnockbackSkillConfig)Definition.Config).WindKnockbackBuffDefinition,
                new Buffs.WindKnockbackBuffArgs(dist, stats.MaxKnockbackDistance.Final, new Buffs.BuffSource(Owner.SelfUnit, this))
            ));
        }
    }
}
