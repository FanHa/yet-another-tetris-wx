using System.Collections.Generic;
using UnityEngine;

namespace Units.Projectiles
{
    public sealed class ElectricFieldZone : PeriodicAreaEffectZoneBase
    {
        private ThunderStrikeProjectile hitProjectilePrefab;
        private float damage;
        private float energyDisruptionAmount;

        public void Initialize(
            Unit caster,
            float radius,
            float duration,
            ThunderStrikeProjectile hitProjectilePrefab,
            float damage,
            float energyDisruptionAmount,
            Skills.Skill sourceSkill)
        {
            InitializeBase(caster, radius, duration, sourceSkill);
            this.hitProjectilePrefab = hitProjectilePrefab;
            this.damage = damage;
            this.energyDisruptionAmount = energyDisruptionAmount;
        }

        protected override IEnumerable<Unit> FindTargetsInArea()
        {
            List<Unit> enemies = UnitManager.FindEnemiesInRangeAtPosition(
                SourceUnit.faction,
                (Vector2)transform.position,
                Radius);

            if (enemies.Count > 0)
            {
                yield return enemies[Random.Range(0, enemies.Count)];
            }
        }

        protected override void ApplyEffectTo(Unit target)
        {
            ThunderStrikeProjectile hitProjectile = Object.Instantiate(
                hitProjectilePrefab,
                target.transform.position,
                Quaternion.identity);
            hitProjectile.Init(SourceUnit, target, damage, energyDisruptionAmount, SourceSkill);
            hitProjectile.Activate();
        }
    }
}