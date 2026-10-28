using System;
using System.Collections.Generic;
using Controller;
using UnityEngine;

namespace Units.Skills
{
    public sealed class SkillTargeting : ISkillTargeting
    {
        private readonly Unit owner;

        public SkillTargeting(Unit owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public IReadOnlyList<Unit> GetActiveEnemies()
        {
            Unit.Faction enemyFaction = owner.faction == Unit.Faction.FactionA
                ? Unit.Faction.FactionB
                : Unit.Faction.FactionA;
            return RequireUnitManager().GetActiveUnits(enemyFaction);
        }

        public IReadOnlyList<Unit> FindEnemiesInRange(float range)
        {
            return RequireUnitManager().FindEnemiesInRange(owner, range);
        }

        public Unit FindRandomAlly() => RequireUnitManager().FindRandomAlly(owner);
        public Unit FindRandomAllyIncludingSelf() => RequireUnitManager().FindRandomAllyIncludingSelf(owner);
        public Unit FindRandomAllyInRange(float range) => RequireUnitManager().FindRandomAllyInRange(owner, range);
        public Unit FindRandomAllyInRangeIncludingSelf(float range) => RequireUnitManager().FindRandomAllyInRangeIncludingSelf(owner, range);

        public Unit FindClosestEnemyInRange(float range)
        {
            return RequireUnitManager().FindClosestEnemyInRange(owner, range);
        }

        public bool TryGetClosestEnemyInAttackRange(out Unit target)
        {
            target = null;
            if (!owner.TryGetClosestEnemy(out var candidate))
            {
                return false;
            }

            if (owner.GetEffectiveAttackRangeTo(candidate) < Vector2.Distance(owner.transform.position, candidate.transform.position))
            {
                return false;
            }

            target = candidate;
            return true;
        }

        private UnitManager RequireUnitManager()
        {
            if (owner.UnitManager == null)
            {
                throw new InvalidOperationException($"Unit '{owner.name}' requires an injected UnitManager for skill targeting queries.");
            }

            return owner.UnitManager;
        }
    }
}
