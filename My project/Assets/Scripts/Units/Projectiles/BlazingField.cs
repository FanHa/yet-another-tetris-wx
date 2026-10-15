using System.Collections.Generic;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class BlazingField : PeriodicAreaEffectZoneBase
    {
        private float dotDps;
        private float dotDuration;
        private const float tickInterval = 1f;

        public void Init(Unit caster, float radius, float duration, float dotDps, float dotDuration, Skills.Skill sourceSkill)
        {
            InitializeBase(caster, radius, duration, tickInterval, sourceSkill);
            this.dotDps = dotDps;
            this.dotDuration = dotDuration;
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
            var burn = BuffFactory.Create(
                ((Model.Skills.BlazingFieldSkillConfig)SourceSkill.Definition.Config).BurnBuffDefinition,
                new BurnBuffArgs(dotDps, dotDuration, new BuffSource(SourceUnit, SourceSkill)));
            target.AddBuff(burn);
        }
    }
}