using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class EnergyCurseZone : PeriodicAreaEffectZoneBase
    {
        private readonly string stackId = System.Guid.NewGuid().ToString("N");
        private EnergyCurseBuffDefinition energyCurseBuffDefinition;
        private int resonantCellCount;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            int resonantCellCount,
            Skills.Skill sourceSkill,
            EnergyCurseBuffDefinition energyCurseBuffDefinition)
        {
            InitializeBase(caster, radius, duration, sourceSkill);
            this.resonantCellCount = resonantCellCount;
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
            target.AddBuff(new EnergyCurseBuff(
                energyCurseBuffDefinition,
                resonantCellCount,
                new BuffSource(SourceUnit, SourceSkill),
                stackId));
        }
    }
}
