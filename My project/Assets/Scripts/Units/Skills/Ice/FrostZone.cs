using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.Skills
{
    public class FrostZone : Skill
    {
        public float BaseRadius = 1.5f;
        public float BaseDuration = 5f;
        public float BaseDamage = 8f;
        public float BaseChilledDuration = 2f;

        public float RadiusPerIceCell = 0.2f;
        public float DurationPerIceCell = 1f;
        public float DamagePerIceCell = 3f;
        public float ChilledDurationPerIceCell = 0.5f;

        public int BaseChilledMoveSlowPercent = 10;
        public int ChilledMoveSlowPercentPerIceCell = 3;
        public int BaseChilledAtkSlowPercent = 10;
        public int ChilledAtkSlowPercentPerIceCell = 3;
        public int BaseChilledEnergySlowPercent = 15;
        public int ChilledEnergySlowPercentPerIceCell = 4;

        private int iceCellCount = 0;

        public FrostZone()
        {
            RequiredEnergy = 120f;
        }

        public void SetIceCellCount(int iceCellCount)
        {
            this.iceCellCount = iceCellCount;
        }

        public float GetRadius()
        {
            return BaseRadius + iceCellCount * RadiusPerIceCell;
        }

        public float GetDuration()
        {
            return BaseDuration + iceCellCount * DurationPerIceCell;
        }

        protected override void ExecuteCore(Unit caster)
        {
            var enemiesInRange = FindEnemiesInRange(caster, caster.Attributes.AttackRange);
            if (enemiesInRange.Count == 0)
                return;

            // 以第一个敌人为中心
            Unit targetEnemy = enemiesInRange.First();
            Vector3 center = targetEnemy.transform.position;

            float radius = GetRadius();
            float duration = GetDuration();
            float damage = BaseDamage + iceCellCount * DamagePerIceCell;
            float chilledDuration = BaseChilledDuration + iceCellCount * ChilledDurationPerIceCell;
            int moveSlowPercent = BaseChilledMoveSlowPercent + iceCellCount * ChilledMoveSlowPercentPerIceCell;
            int atkSlowPercent = BaseChilledAtkSlowPercent + iceCellCount * ChilledAtkSlowPercentPerIceCell;
            int energySlowPercent = BaseChilledEnergySlowPercent + iceCellCount * ChilledEnergySlowPercentPerIceCell;

            TriggerEffect(new SkillEffectContext
            {
                Skill = this,
                Position = center,
                // Duration = duration,
                // Radius = radius
            });
            caster.StartCoroutine(FrostZoneRoutine(caster, center, radius, duration, damage, chilledDuration, moveSlowPercent, atkSlowPercent, energySlowPercent));
        }

        private IEnumerator FrostZoneRoutine(Unit caster, Vector3 center, float radius, float duration, float damage, float chilledDuration, int moveSlowPercent, int atkSlowPercent, int energySlowPercent)
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
                        // 造成冰属性伤害
                        var iceDamage = new Damages.Damage(damage, Damages.DamageType.Ice);
                        iceDamage.SetSourceUnit(caster);
                        iceDamage.SetSourceLabel("霜域");
                        iceDamage.SetTargetUnit(enemy);
                        enemy.TakeDamage(iceDamage);

                        // 施加Chilled Buff
                        var chilled = new Units.Buffs.Chilled(
                            chilledDuration,
                            moveSlowPercent,
                            atkSlowPercent,
                            energySlowPercent,
                            caster,
                            this
                        );
                        enemy.AddBuff(chilled);
                    }
                }
                yield return new WaitForSeconds(tickInterval);
                elapsed += tickInterval;
            }
        }

        public override string Name() => "霜域";
        public override string Description() =>
            $"在目标区域制造一片极寒地带，{BaseDuration}+{DurationPerIceCell}*冰系Cell秒内每秒对范围内敌人造成冰属性伤害并施加Chilled，所有效果随冰系Cell提升。";
    }
}