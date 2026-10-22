using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class PowerCurseZone : PeriodicAreaEffectZoneBase
    {
        private readonly string stackId = System.Guid.NewGuid().ToString("N");
        private PowerCurseBuffDefinition powerCurseBuffDefinition;
        private int resonantCellCount;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            int resonantCellCount,
            Skills.Skill sourceSkill,
            PowerCurseBuffDefinition powerCurseBuffDefinition)
        {
            InitializeBase(caster, radius, duration, sourceSkill);
            this.resonantCellCount = resonantCellCount;
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
            target.AddBuff(new PowerCurseBuff(
                powerCurseBuffDefinition,
                resonantCellCount,
                new BuffSource(SourceUnit, SourceSkill),
                stackId));
        }
    }
}