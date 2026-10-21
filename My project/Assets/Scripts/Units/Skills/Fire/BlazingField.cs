using System.Collections.Generic;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    public class BlazingField : ActiveSkill
    {
        public BlazingFieldLevelConfig Config { get; }
        private Vector3 targetPosition;
        private readonly HashSet<Units.Projectiles.BlazingField> spawnedFields = new();

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
        }

        private int GetResonantCellCount()
        {
            return Owner != null ? Owner.GetResonanceCount(Definition.Resonance) : 0;
        }

        private BlazingFieldStats CalcStats()
        {
            int resonantCellCount = GetResonantCellCount();
            return new BlazingFieldStats
            {
                Radius = new StatValue("范围半径", Config.BaseRadius, resonantCellCount * Config.RadiusPerResonantCell),
                Duration = new StatValue("持续时间", Config.BaseDuration, resonantCellCount * Config.DurationPerResonantCell)
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
            spawnedFields.RemoveWhere(field => field == null);

            var effect = Object.Instantiate(
                SkillConfig.ProjectilePrefab,
                targetPosition,
                Quaternion.identity
            );
            spawnedFields.Add(effect);
            effect.Init(
                caster: Owner.SelfUnit,
                radius: stats.Radius.Final,
                duration: stats.Duration.Final,
                sourceSkill: this,
                resonantCellCount: GetResonantCellCount()
            );
            effect.Activate();
        }

        public override void OnRelease()
        {
            foreach (var effect in spawnedFields)
            {
                if (effect != null)
                {
                    Object.Destroy(effect.gameObject);
                }
            }

            spawnedFields.Clear();
        }


        public override string Description()
        {
            var stats = CalcStats();
            return
                DescriptionStatic() + ":\n" +
                $"{stats.Radius}\n" +
                $"{stats.Duration}\n" +
                SkillConfig.ProjectilePrefab.DescribeEffect(GetResonantCellCount());
        }

        public static string DescriptionStatic() => "制造一片燃烧区域, 对敌人造成伤害.";

        public override string Name()
        {
            return NameStatic();
        }
        public static string NameStatic() => "焰域";
    }
}