using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class FrostZone : PeriodicAreaEffectZoneBase
    {
        [SerializeField] private ChilledBuffDefinition chilledDefinition;
        private float damage;
        private float chilledDuration;
        private int moveSlowPercent;
        private int atkSlowPercent;
        private int actionSlowPercent;
        private BuffSource chilledSource;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            float damage,
            Units.Skills.Skill sourceSkill,
            int resonantCellCount
        )
        {
            InitializeBase(
                caster: caster,
                radius: radius,
                duration: duration,
                sourceSkill: sourceSkill);
            this.damage = damage;
            var stats = chilledDefinition.CalculateStats(resonantCellCount);
            chilledDuration = stats.Duration.FinalValue;
            moveSlowPercent = Mathf.RoundToInt(stats.MoveSlowPercent.FinalValue);
            atkSlowPercent = Mathf.RoundToInt(stats.AttackSlowPercent.FinalValue);
            actionSlowPercent = Mathf.RoundToInt(stats.ActionSlowPercent.FinalValue);
            chilledSource = new BuffSource(caster, sourceSkill);
        }

        public string DescribeEffect(int resonantCellCount)
        {
            return chilledDefinition.DescribeEffect(resonantCellCount);
        }

        protected override IEnumerable<Unit> FindTargetsInArea()
        {
            return UnitManager.FindEnemiesInRangeAtPosition(
                SourceUnit.faction,
                (Vector2)transform.position,
                Radius);
        }

        protected override void ApplyEffectTo(Unit target)
        {
            var iceDamage = new Damages.Damage(damage, Damages.DamageType.Skill);
            iceDamage.SetSourceUnit(SourceUnit);
            iceDamage.SetSourceLabel(SourceSkill.Name());
            iceDamage.SetTargetUnit(target);
            target.TakeDamage(iceDamage);

            var chilled = new Chilled(
                chilledDefinition,
                BuffDuration.ForSeconds(chilledDuration),
                moveSlowPercent,
                atkSlowPercent,
                actionSlowPercent,
                chilledSource);
            target.AddBuff(chilled);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (chilledDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(chilledDefinition)} is not assigned.", this);
            }
        }
#endif

    }
}