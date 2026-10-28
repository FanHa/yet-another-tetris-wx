using System.Collections.Generic;
using Model.Buffs;
using Units.Buffs;
using UnityEngine;

namespace Units.Projectiles
{
    public class PowerCurseZone : PeriodicAreaEffectZoneBase
    {
        private readonly string stackId = System.Guid.NewGuid().ToString("N");
        [SerializeField] private PowerCurseBuffDefinition powerCurseBuffDefinition;
        private int resonantCellCount;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            int resonantCellCount,
            Skills.Skill sourceSkill)
        {
            InitializeBase(caster, radius, duration, sourceSkill);
            this.resonantCellCount = resonantCellCount;
        }

        public string DescribeEffect(int resonantCellCount) => powerCurseBuffDefinition.DescribeEffect(resonantCellCount);

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

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (powerCurseBuffDefinition == null)
            {
                Debug.LogError($"{GetType().Name} '{name}'.{nameof(powerCurseBuffDefinition)} is not assigned.", this);
            }
            else if (powerCurseBuffDefinition.Id != "buff.power_curse")
            {
                Debug.LogError($"{nameof(powerCurseBuffDefinition)} must reference the 'buff.power_curse' definition.", this);
            }
        }
#endif
    }
}