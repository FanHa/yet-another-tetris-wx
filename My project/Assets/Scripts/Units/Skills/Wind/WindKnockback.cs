using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class WindKnockback : Skill, IPassiveSkill
    {
        public override CellTypeId CellTypeId => CellTypeId.WindKnockback;

        public WindKnockbackConfig Config { get; }

        public WindKnockback(WindKnockbackConfig config)
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
            int windCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Wind, out var count) ? count : 0;
            return new WindKnockbackStats
            {
                KnockbackDistance = new StatValue("击退距离", Config.BaseKnockbackDistance, windCellCount * Config.KnockbackDistancePerWindCell),
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

        public void ApplyPassive()
        {
            var stats = CalcStats();
            float dist = Mathf.Min(stats.KnockbackDistance.Final, stats.MaxKnockbackDistance.Final);
            Owner.AddBuff(new Buffs.WindKnockbackBuff(
                knockbackDistance: dist,
                maxKnockbackDistance: stats.MaxKnockbackDistance.Final,
                sourceUnit: Owner,
                sourceSkill: this
            ));
        }
    }

    [CreateAssetMenu(menuName = "SkillConfig/WindKnockbackConfigGroup")]
    public class WindKnockbackConfigGroup : SkillConfigGroup
    {
        public List<WindKnockbackConfig> LevelConfigs;
    }

    [System.Serializable]
    public class WindKnockbackConfig : SkillConfig
    {
        [Header("属性")]
        public float BaseKnockbackDistance = 0.12f;
        public float KnockbackDistancePerWindCell = 0.01f;
        public float MaxKnockbackDistance = 0.18f;
    }
}
