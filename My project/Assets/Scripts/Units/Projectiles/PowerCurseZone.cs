using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class PowerCurseZone : PeriodicAreaEffectZoneBase
    {
        private readonly string stackId = System.Guid.NewGuid().ToString("N");
        private BuffDefinition powerCurseBuffDefinition;
        private float debuffDuration;
        private float attackReductionPercent;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            float applyInterval,
            float debuffDuration,
            float attackReductionPercent,
            Skills.Skill sourceSkill,
            BuffDefinition powerCurseBuffDefinition)
        {
            InitializeBase(caster, radius, duration, applyInterval, sourceSkill);
            this.debuffDuration = debuffDuration;
            this.attackReductionPercent = attackReductionPercent;
            this.powerCurseBuffDefinition = powerCurseBuffDefinition;
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
            target.AddBuff(BuffFactory.Create(
                powerCurseBuffDefinition,
                new PowerCurseBuffArgs(
                    debuffDuration,
                    attackReductionPercent,
                    new BuffSource(SourceUnit, SourceSkill),
                    stackId)));
        }
    }
}