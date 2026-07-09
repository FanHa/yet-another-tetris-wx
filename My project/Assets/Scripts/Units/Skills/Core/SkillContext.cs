using System;
using System.Collections.Generic;
using Controller;
using Model.Tetri;
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

        public Unit SelfUnit => unit;
        public bool IsActive => unit.IsActive;
        public bool IsStunned => unit.IsStunned;
        public bool IsSkillMotionActive => unit.IsSkillMotionActive;
        public float ActionSpeed => unit.ActionSpeed;

        public Attributes Attributes => unit.Attributes;
        public Dictionary<AffinityType, int> CellCounts => unit.CellCounts;
        public Model.ProjectileConfig ProjectileConfig => unit.ProjectileConfig;
        public Transform transform => unit.transform;
        public string name => unit.name;
        public Transform projectileSpawnPoint => unit.projectileSpawnPoint;

        public Coroutine StartCoroutine(System.Collections.IEnumerator routine)
        {
            return unit.StartCoroutine(routine);
        }

        public List<Unit> FindEnemiesInRange(float range)
        {
            return RequireUnitManager().FindEnemiesInRange(unit, range);
        }

        public Unit FindRandomAlly(float range, bool includeSelf = false)
        {
            return RequireUnitManager().FindRandomAlly(unit, range, includeSelf);
        }

        public Unit FindClosestEnemyInRange(float range)
        {
            return RequireUnitManager().FindClosestEnemyInRange(unit, range);
        }

        public Unit FindLowestMaxHealthEnemy()
        {
            return RequireUnitManager().FindLowestMaxHealthEnemy(unit);
        }

        public Unit FindFurthestEnemy()
        {
            return RequireUnitManager().FindFurthestEnemy(unit);
        }

        public bool TryGetClosestEnemyInAttackRange(out Unit target)
        {
            target = null;
            if (!unit.TryGetClosestEnemy(out var candidate))
            {
                return false;
            }

            if (GetEffectiveAttackRangeTo(candidate) < Vector2.Distance(unit.transform.position, candidate.transform.position))
            {
                return false;
            }

            target = candidate;
            return true;
        }

        public bool TryGetClosestEnemy(out Unit target)
        {
            return RequireUnitManager().FindClosestEnemy(unit) is { } closestEnemy
                ? (target = closestEnemy) != null
                : (target = null) == null;
        }

        public bool TryGetClosestAlly(out Unit target)
        {
            return RequireUnitManager().FindClosestAlly(unit) is { } closestAlly
                ? (target = closestAlly) != null
                : (target = null) == null;
        }

        private float GetEffectiveAttackRangeTo(Unit target)
        {
            return unit.Attributes.AttackRange.finalValue + unit.BodyRadius + target.BodyRadius;
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