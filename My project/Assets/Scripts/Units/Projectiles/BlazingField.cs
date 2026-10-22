using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class BlazingField : PeriodicAreaEffectZoneBase
    {
        [SerializeField] private BurnBuffDefinition burnDefinition;
        private float burnDps;
        private float burnDuration;
        private BuffSource burnSource;

        public void Init(Unit caster, float radius, float duration, Skills.Skill sourceSkill, int resonantCellCount)
        {
            InitializeBase(caster, radius, duration, sourceSkill);
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            burnDps = stats.Dps.Final;
            burnDuration = stats.Duration.Final;
            burnSource = new BuffSource(caster, sourceSkill);
        }

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            return $"{stats.Dps}\n{stats.Duration}";
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
            var burn = new Burn(burnDefinition, burnDps, BuffDuration.ForSeconds(burnDuration), burnSource);
            target.AddBuff(burn);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (burnDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(burnDefinition)} is not assigned.", this);
            }
        }
#endif
    }
}