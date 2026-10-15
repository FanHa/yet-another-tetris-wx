using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class FlameRing : PeriodicAreaEffectZoneBase
    {
        private BuffDefinition burnDefinition;
        private float dotDps;
        private float dotDuration;

        public void Initialize(
            Unit owner,
            float radius,
            Skills.Skill sourceSkill,
            BuffDefinition burnDefinition,
            float duration,
            float dotDps,
            float dotDuration
        )
        {
            InitializeBase(owner, radius, duration, sourceSkill);
            this.burnDefinition = burnDefinition;
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;
        }

        protected override void UpdateZonePosition()
        {
            transform.position = SourceUnit.transform.position;
        }

        protected override IEnumerable<Unit> FindTargetsInArea()
        {
            if (UnitManager == null)
                return System.Array.Empty<Unit>();

            return UnitManager.FindEnemiesInRangeAtPosition(
                SourceUnit.faction,
                (Vector2)transform.position,
                Radius);
        }

        protected override void ApplyEffectTo(Unit target)
        {
            target.AddBuff(BuffFactory.Create(
                burnDefinition,
                new BurnBuffArgs(dotDps, dotDuration, new BuffSource(SourceUnit, SourceSkill))));
        }
    }
}