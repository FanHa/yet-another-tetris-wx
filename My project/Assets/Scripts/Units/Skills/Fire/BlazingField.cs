using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class BlazingField : ActiveSkill
    {
        public BlazingFieldLevelConfig Config { get; }
        private Vector3 targetPosition;

        private BlazingFieldSkillConfig SkillConfig => (BlazingFieldSkillConfig)Definition.Config;

        public BlazingField(BlazingFieldLevelConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        private struct BlazingFieldStats
        {
            public StatValue Radius;
            public StatValue Duration;
            public StatValue DotDps;
            public StatValue DotDuration;
        }

        private BlazingFieldStats CalcStats()
        {
            int fireCellCount = Owner != null && Owner.CellCounts.TryGetValue(AffinityType.Fire, out var count) ? count : 0;
            return new BlazingFieldStats
            {
                Radius = new StatValue("范围半径", Config.BaseRadius, fireCellCount * Config.RadiusPerFireCell),
                Duration = new StatValue("持续时间", Config.BaseDuration, fireCellCount * Config.DurationPerFireCell),
                DotDps = new StatValue("每秒伤害", Config.BaseDotDps, fireCellCount * Config.DotDpsPerFireCell),
                DotDuration = new StatValue("灼烧持续时间", Config.BaseDotDuration, fireCellCount * Config.DotDurationPerFireCell)
            };
        }

        protected override bool PrepareCastCore()
        {
            // 使用与 AttackAction 相同的有效射程（含 AgentRadius）查找敌人
            if (!Owner.TryGetClosestEnemyInAttackRange(out var targetEnemy))
                return false;
            targetPosition = targetEnemy.transform.position;
            return true;

        }

        protected override void ExecuteCore()
        {
            var stats = CalcStats();

            var effect = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                targetPosition,
                Quaternion.identity
            );
            effect.Init(
                caster: Owner.SelfUnit,
                radius: stats.Radius.Final,
                duration: stats.Duration.Final,
                dotDps: stats.DotDps.Final,
                dotDuration: stats.DotDuration.Final,
                sourceSkill: this
            );
            effect.Activate();
        }


        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Radius}\n" +
                $"{stats.Duration}\n" +
                $"{stats.DotDps}\n" +
                $"{stats.DotDuration}";
        }

        public static string DescriptionStatic() => "制造一片燃烧区域, 对敌人造成伤害.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "焰域";
    }
}