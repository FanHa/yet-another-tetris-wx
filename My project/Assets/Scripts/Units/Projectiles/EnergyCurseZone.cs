using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class EnergyCurseZone : PeriodicAreaEffectZoneBase
    {
        private readonly string stackId = System.Guid.NewGuid().ToString("N");
        private BuffDefinition energyCurseBuffDefinition;
        private float debuffDuration;
        private float energyRegenReductionPercent;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            float debuffDuration,
            float energyRegenReductionPercent,
            Skills.Skill sourceSkill,
            BuffDefinition energyCurseBuffDefinition)
        {
            InitializeBase(caster, radius, duration, sourceSkill);
            this.debuffDuration = debuffDuration;
            this.energyRegenReductionPercent = energyRegenReductionPercent;
            this.energyCurseBuffDefinition = energyCurseBuffDefinition;
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
                energyCurseBuffDefinition,
                new EnergyCurseBuffArgs(
                    debuffDuration,
                    energyRegenReductionPercent,
                    new BuffSource(SourceUnit, SourceSkill),
                    stackId)));
        }
    }
}
