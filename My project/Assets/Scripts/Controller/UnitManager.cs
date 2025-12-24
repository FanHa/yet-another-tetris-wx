using System.Collections.Generic;
using UnityEngine;
using Units;
using Model.Tetri;
using Model;
using System;
using Units.Skills;
using System.Linq;

namespace Controller
{
    public class UnitManager : MonoBehaviour
    {
        public event Action<Unit> OnUnitClicked;
        public event Action<Unit> OnUnitDeath;
        public event Action<Unit.Faction> OnFactionAllDead;
        public event Action<Units.Damages.Damage> OnUnitDamageTaken;
        public event Action<Units.Unit, Units.Skills.Skill> OnGlobalSkillCast;

        [SerializeField] private UnitFactory unitFactory;
        [SerializeField] private Transform factionARoot;
        [SerializeField] private Transform factionBRoot;
        private List<Unit> factionA = new();
        private List<Unit> factionB = new();

        private const float RandomOffsetRangeX = 5f;
        private const float RandomOffsetRangeY = 0.5f;

        /// <summary>
        /// 批量生成单位
        /// </summary>
        /// <param name="items">单位配置列表</param>
        /// <param name="spawnPoint">出生点</param>
        /// <param name="faction">阵营</param>
        /// <param name="parent">父物体</param>
        public void SpawnUnits(
            List<CharacterPlacement> items,
            Transform spawnPoint,
            Unit.Faction faction,
            Transform minBounds,
            Transform maxBounds
        )
        {
            foreach (Model.CharacterPlacement item in items)
            {
                Unit unit = unitFactory.CreateUnit(item.CharacterInfluence);
                unit.transform.SetParent(GetRootByFaction(faction), false);

                Vector3 spawnPos = spawnPoint.position + item.RelativePositionFromCenter;

                unit.transform.position = spawnPos;
                unit.OnDeath += HandleUnitDeath;
                unit.OnDamageTaken += HandleDamageTaken;

                unit.OnSkillCast += HandleSkillCast;
                unit.OnClicked += HandleUnitClicked;
                unit.UnitManager = this;
                unit.Setup(faction, minBounds, maxBounds);
                if (unit.faction == Unit.Faction.FactionA)
                {
                    factionA.Add(unit);
                }
                if (unit.faction == Unit.Faction.FactionB)
                {
                    factionB.Add(unit);
                }
            }
        }

        public void ActivateAllUnits()
        {
            foreach (var unit in factionA.Concat(factionB))
            {
                if (unit != null && !unit.IsActive)
                {
                    unit.Activate();
                }
            }
        }

        private void HandleUnitClicked(Unit unit)
        {
            OnUnitClicked?.Invoke(unit);
        }

        public void Reset()
        {
            // 遍历 factionAParent 的所有子对象并销毁
            foreach (Transform child in factionARoot)
            {
                Destroy(child.gameObject);
            }

            // 遍历 factionBRoot 的所有子对象并销毁
            foreach (Transform child in factionBRoot)
            {
                Destroy(child.gameObject);
            }

            factionA.Clear();
            factionB.Clear();
        }

        private Transform GetRootByFaction(Unit.Faction faction)
        {
            return faction == Unit.Faction.FactionA ? factionARoot : factionBRoot;
        }


        private void HandleUnitDeath(Unit deadUnit)
        {
            factionA.Remove(deadUnit);
            factionB.Remove(deadUnit);
            OnUnitDeath?.Invoke(deadUnit);
            List<Unit> faction = deadUnit.faction == Unit.Faction.FactionA ? factionA : factionB;
            if (faction.Count == 0)
            {
                foreach (Unit unit in factionA.Concat(factionB).ToList())
                {
                    unit.Deactivate();
                }
                OnFactionAllDead?.Invoke(deadUnit.faction);
            }

        }

        private void HandleDamageTaken(Units.Damages.Damage damage)
        {
            OnUnitDamageTaken?.Invoke(damage);
        }

        private void HandleSkillCast(Units.Unit unit, Units.Skills.Skill skill)
        {
            OnGlobalSkillCast?.Invoke(unit, skill);
        }


        internal List<Unit> GetFactionBUnits()
        {
            return factionB;
        }

        internal List<Unit> GetFactionAUnits()
        {
            return factionA;
        }

        public List<Unit> GetUnitsByFaction(Unit.Faction faction)
        {
            return faction == Unit.Faction.FactionA ? factionA : factionB;
        }

        public List<Unit> FindAlliesInRange(Unit self, float range, bool includeSelf = false)
        {
            var list = self.faction == Unit.Faction.FactionA ? factionA : factionB;
            float r2 = range * range;
            Vector2 selfPos = self.transform.position;

            return list
                .Where(u => u != null && u.IsActive && (includeSelf || u != self))
                .Where(u => ((Vector2)u.transform.position - selfPos).sqrMagnitude <= r2)
                .ToList();
        }

        public List<Unit> FindEnemiesInRange(Unit self, float range)
        {
            var list = self.faction == Unit.Faction.FactionA ? factionB : factionA;
            float r2 = range * range;
            Vector2 selfPos = self.transform.position;

            return list
                .Where(u => u != null && u.IsActive)
                .Where(u => ((Vector2)u.transform.position - selfPos).sqrMagnitude <= r2)
                .ToList();
        }

        public Unit FindRandomEnemyInRange(Unit self, float range)
        {
            var enemies = FindEnemiesInRange(self, range);
            if (enemies.Count == 0) return null;
            return enemies[UnityEngine.Random.Range(0, enemies.Count)];
        }

        public Unit FindClosestEnemyInRange(Unit self, float range)
        {
            var list = self.faction == Unit.Faction.FactionA ? factionB : factionA;
            float r2 = range * range;
            Vector2 selfPos = self.transform.position;

            return list
                .Where(u => u != null && u.IsActive)
                .Where(u => ((Vector2)u.transform.position - selfPos).sqrMagnitude <= r2)
                .OrderBy(u => ((Vector2)u.transform.position - selfPos).sqrMagnitude)
                .FirstOrDefault();
        }


        public Unit FindRandomAlly(Unit self, float range, bool includeSelf = false)
        {
            var allies = FindAlliesInRange(self, range, includeSelf);
            if (allies.Count == 0) return null;
            return allies[UnityEngine.Random.Range(0, allies.Count)];
        }

        public List<Unit> FindEnemiesInRangeAtPosition(Unit.Faction selfFaction, Vector2 center, float range)
        {
            var list = selfFaction == Unit.Faction.FactionA ? factionB : factionA;
            float r2 = range * range;
            return list
                .Where(u => u != null && u.IsActive)
                .Where(u => ((Vector2)u.transform.position - center).sqrMagnitude <= r2)
                .ToList();
        }

        public Unit FindWeakestEnemy(Unit self)
        {
            var enemyList = self.faction == Unit.Faction.FactionA ? factionB : factionA;

            return enemyList
                .Where(u => u != null && u.IsActive)
                .OrderBy(u => u.Attributes.MaxHealth.finalValue)
                .FirstOrDefault();
        }
        
        public Unit FindFurthestEnemy(Unit self)
        {
            var enemyList = self.faction == Unit.Faction.FactionA ? factionB : factionA;
            Vector2 selfPos = self.transform.position;

            return enemyList
                .Where(u => u != null && u.IsActive)
                .OrderByDescending(u => ((Vector2)u.transform.position - selfPos).sqrMagnitude)
                .FirstOrDefault();
        }

        public List<Unit> GetAllUnits()
        {
            // 返回所有阵营的活动单位
            return factionA.Concat(factionB)
                .Where(u => u != null && u.IsActive)
                .ToList();
        }
    }
}