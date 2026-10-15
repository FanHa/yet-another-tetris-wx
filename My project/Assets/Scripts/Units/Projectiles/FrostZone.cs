using System.Collections.Generic;
using UnityEngine;

namespace Units.Projectiles
{
    public class FrostZone : PeriodicAreaEffectZoneBase
    {
        private float damage;
        private float chilledDuration;
        private int moveSlowPercent;
        private int atkSlowPercent;
        private int actionSlowPercent;
        private int energySlowPercent;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            float damage,
            float chilledDuration,
            int moveSlowPercent,
            int atkSlowPercent,
            int actionSlowPercent,
            int energySlowPercent,
            Units.Skills.Skill sourceSkill
        )
        {
            InitializeBase(
                caster: caster,
                radius: radius,
                duration: duration,
                sourceSkill: sourceSkill);
            this.damage = damage;
            this.chilledDuration = chilledDuration;
            this.moveSlowPercent = moveSlowPercent;
            this.atkSlowPercent = atkSlowPercent;
            this.actionSlowPercent = actionSlowPercent;
            this.energySlowPercent = energySlowPercent;
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

            var chilled = Units.Buffs.BuffFactory.Create(
                ((Model.Skills.FrostZoneSkillConfig)SourceSkill.Definition.Config).ChilledBuffDefinition,
                new Units.Buffs.ChilledBuffArgs(
                    chilledDuration,
                    moveSlowPercent,
                    atkSlowPercent,
                    actionSlowPercent,
                    energySlowPercent,
                    new Units.Buffs.BuffSource(SourceUnit, SourceSkill)));
            target.AddBuff(chilled);
        }
    }
}