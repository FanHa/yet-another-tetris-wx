using System;
using System.Collections.Generic;
using Controller;
using Model.Resonance;
using Model.Skills;
using UnityEngine;

namespace Units.Skills
{
    /// <summary>
    /// 技能和技能处理器依赖的上下文适配器。
    /// 负责把 Unit 暴露成技能所需的最小能力集合。
    /// </summary>
    public sealed class SkillContext : ISkillContext
    {
        private readonly Unit unit;

        public SkillContext(Unit unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        public Unit Unit => unit;

        public Attributes Attributes => unit.Attributes;
        public int GetResonanceCount(ResonanceType type) => unit.GetResonanceCount(type);
        public Transform Transform => unit.transform;
        public string Name => unit.name;
        public Transform ProjectileSpawnPoint => unit.projectileSpawnPoint;


        public IReadOnlyList<Unit> GetActiveEnemies()
        {
            Unit.Faction enemyFaction = unit.faction == Unit.Faction.FactionA
                ? Unit.Faction.FactionB
                : Unit.Faction.FactionA;
            return RequireUnitManager().GetActiveUnits(enemyFaction);
        }

        public IReadOnlyList<Unit> FindEnemiesInRange(float range)
        {
            return RequireUnitManager().FindEnemiesInRange(unit, range);
        }

        public Unit FindRandomAlly() => RequireUnitManager().FindRandomAlly(unit);
        public Unit FindRandomAllyIncludingSelf() => RequireUnitManager().FindRandomAllyIncludingSelf(unit);
        public Unit FindRandomAllyInRange(float range) => RequireUnitManager().FindRandomAllyInRange(unit, range);
        public Unit FindRandomAllyInRangeIncludingSelf(float range) => RequireUnitManager().FindRandomAllyInRangeIncludingSelf(unit, range);

        public Unit FindClosestEnemyInRange(float range)
        {
            return RequireUnitManager().FindClosestEnemyInRange(unit, range);
        }

        public bool TryGetClosestEnemyInAttackRange(out Unit target)
        {
            target = null;
            if (!unit.TryGetClosestEnemy(out var candidate))
            {
                return false;
            }

            if (unit.GetEffectiveAttackRangeTo(candidate) < Vector2.Distance(unit.transform.position, candidate.transform.position))
            {
                return false;
            }

            target = candidate;
            return true;
        }

        public void SetMoveBehaviorMode(Unit.MoveBehaviorMode mode)
        {
            unit.SetMoveBehaviorMode(mode);
        }

        public void Teleport(Vector3 position)
        {
            unit.Teleport(position);
        }

        public Units.Movement.MovementResult MoveBy(Vector3 delta)
        {
            return unit.MoveBy(delta);
        }

        public void AddBuff(Units.Buffs.Buff buff)
        {
            unit.AddBuff(buff);
        }

        private UnitManager RequireUnitManager()
        {
            if (unit.UnitManager == null)
            {
                throw new InvalidOperationException($"Unit '{unit.name}' requires an injected UnitManager for skill context queries.");
            }

            return unit.UnitManager;
        }
    }
}