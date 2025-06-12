using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class BlazingField : Skill
    {
        public float BaseRadius = 1.5f;
        public float BaseDuration = 5f;
        public float BaseDotDps = 5f;
        public float BaseDotDuration = 3f;

        public float RadiusPerFireCell = 0.2f;
        public float DurationPerFireCell = 1f;
        public float DotDpsPerFireCell = 2f;
        public float DotDurationPerFireCell = 0.5f;

        private int fireCellCount = 0;

        public BlazingField()
        {
            RequiredEnergy = 120f; // 设置技能所需能量
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

            float radius = BaseRadius + fireCellCount * RadiusPerFireCell;
            float duration = BaseDuration + fireCellCount * DurationPerFireCell;
            float dotDps = BaseDotDps + fireCellCount * DotDpsPerFireCell;
            float dotDuration = BaseDotDuration + fireCellCount * DotDurationPerFireCell;

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
            $"在目标区域制造一片燃烧地带，{BaseDuration}+{DurationPerFireCell}*火系Cell秒内每秒对范围内敌人施加灼烧，伤害和范围随火系Cell提升。";
    }
}