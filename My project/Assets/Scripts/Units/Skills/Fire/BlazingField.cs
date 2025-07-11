using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Units.Skills
{
    public class BlazingField : Skill
    {
        public override CellTypeId CellTypeId => CellTypeId.BlazingField;
        public BlazingFieldConfig Config { get; }
        private int fireCellCount = 0;

        public BlazingField(BlazingFieldConfig config)
        {
            Config = config;
            RequiredEnergy = config.RequiredEnergy;
        }

        public void SetFireCellCount(int fireCellCount)
        {
            this.fireCellCount = fireCellCount;
        }

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange);
            if (enemiesInRange.Count == 0)
                return;

            // 以第一个敌人为中心
            Unit targetEnemy = enemiesInRange.First();
            Vector3 center = targetEnemy.transform.position;

            float radius = Config.BaseRadius + fireCellCount * Config.RadiusPerFireCell;
            float duration = Config.BaseDuration + fireCellCount * Config.DurationPerFireCell;
            float dotDps = Config.BaseDotDps + fireCellCount * Config.DotDpsPerFireCell;
            float dotDuration = Config.BaseDotDuration + fireCellCount * Config.DotDurationPerFireCell;

            TriggerEffect(new SkillEffectContext {
                Skill = this, Position = center, Duration = duration, Radius = radius
            });
            caster.StartCoroutine(BlazingFieldRoutine(caster, center, radius, duration, dotDps, dotDuration));
        }

        private IEnumerator BlazingFieldRoutine(Unit caster, Vector3 center, float radius, float duration, float dotDps, float dotDuration)
        {
            float elapsed = 0f;
            float tickInterval = 1f;

            while (elapsed < duration)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);
                foreach (var collider in colliders)
                {
                    Unit enemy = collider.GetComponent<Unit>();
                    if (enemy != null && enemy.faction != caster.faction)
                    {
                        var dot = new Units.Dot(
                            Units.DotType.Burn,
                            this,
                            caster,
                            dotDps,
                            dotDuration,
                            "焰域灼烧"
                        );
                        enemy.ApplyDot(dot);
                    }
                }
                yield return new WaitForSeconds(tickInterval);
                elapsed += tickInterval;
            }
        }

        public override string Name() => "焰域";
        public override string Description() =>
            $"在目标区域制造一片燃烧地带，{Config.BaseDuration}+{Config.DurationPerFireCell}*火系Cell秒内每秒对范围内敌人施加灼烧，伤害和范围随火系Cell提升。";
    }
}