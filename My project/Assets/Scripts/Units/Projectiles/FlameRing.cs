using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class FlameRing : PeriodicAreaEffectZoneBase
    {
        [SerializeField] private BurnBuffDefinition burnDefinition;
        private float burnDps;
        private float burnDuration;
        private BuffSource burnSource;

        public void Initialize(
            Unit owner,
            float radius,
            Skills.Skill sourceSkill,
            float duration,
            int resonantCellCount
        )
        {
            InitializeBase(owner, radius, duration, sourceSkill);
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            burnDps = stats.Dps.Final;
            burnDuration = stats.Duration.Final;
            burnSource = new BuffSource(owner, sourceSkill);
        }

        public string DescribeEffect(int resonantCellCount)
        {
            var stats = burnDefinition.CalculateStats(resonantCellCount);
            return $"{stats.Dps}\n{stats.Duration}";
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
            target.AddBuff(new Burn(burnDefinition, burnDps, burnDuration, burnSource));
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