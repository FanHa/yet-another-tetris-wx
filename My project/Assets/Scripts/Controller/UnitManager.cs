using System.Collections.Generic;
using UnityEngine;
using Units;
using Model;
using System;
using System.Linq;

namespace Controller
{
    public class UnitManager : MonoBehaviour
    {
        public event Action<Unit> OnUnitClicked;
        public event Action<Unit> OnUnitDeath;
        public event Action<Unit.Faction> OnFactionAllDead;
        public event Action<Units.Damages.Damage> OnUnitDamageTaken;
        public event Action<Units.Unit, Units.Skills.Skill> OnAnyUnitSkillCast;

        [SerializeField] private UnitFactory unitFactory;
        [SerializeField] private Transform factionARoot;
        [SerializeField] private Transform factionBRoot;
        private List<Unit> factionA = new();
        private List<Unit> factionB = new();
        private HashSet<Unit> ownedUnits = new();
        private int unitBatchVersion;
        private bool isActivatingUnits;
        private bool isActivationCancelled;
        private bool isDeactivatingUnits;
        private bool isReleasingUnits;

        /// <summary>
        /// 批量生成单位
        /// </summary>
        /// <param name="items">单位配置列表</param>
        /// <param name="spawnPoint">出生点</param>
        /// <param name="faction">阵营</param>
        public void SpawnUnits(
            IReadOnlyList<CharacterPlacement> items,
            Transform spawnPoint,
            Unit.Faction faction
        )
        {
            foreach (Model.CharacterPlacement item in items)
            {
                Unit unit = unitFactory.CreateBattleUnit(item.CharacterInfluence);
                unit.transform.SetParent(GetRootByFaction(faction), false);

                Vector3 spawnPos = spawnPoint.position + item.RelativePositionFromCenter;

                unit.PlaceAt(spawnPos);
                unit.InjectUnitManager(this);
                unit.Setup(faction);
                RegisterUnit(unit);
            }
        }

        private void RegisterUnit(Unit unit)
        {
            ownedUnits.Add(unit);
            GetFactionUnits(unit.faction).Add(unit);
            unit.OnDeath += HandleUnitDeath;
            unit.OnDamageTaken += HandleDamageTaken;
            unit.OnSkillCast += HandleUnitSkillCast;
            unit.OnClicked += HandleUnitClicked;
        }

        private void UnsubscribeUnit(Unit unit)
        {
            unit.OnDeath -= HandleUnitDeath;
            unit.OnDamageTaken -= HandleDamageTaken;
            unit.OnSkillCast -= HandleUnitSkillCast;
            unit.OnClicked -= HandleUnitClicked;
        }

        private List<Unit> GetFactionUnits(Unit.Faction faction)
        {
            return faction == Unit.Faction.FactionA ? factionA : factionB;
        }

        public void ActivateAllUnits()
        {
            Unit[] unitsToActivate = factionA.Concat(factionB).ToArray();

            foreach (Unit unit in unitsToActivate)
            {
                unit.Activate();
            }

        }

        private void HandleUnitClicked(Unit unit)
        {
            OnUnitClicked?.Invoke(unit);
        }

        public void DeactivateAllUnits()
        {
            Unit[] unitsToDeactivate = ownedUnits.ToArray();

            foreach (Unit unit in unitsToDeactivate)
            {
                unit.Deactivate();
            }
        }
        
        public void ReleaseAllUnits()
        {
            Unit[] unitsToRelease = ownedUnits.ToArray();
            foreach (Unit unit in unitsToRelease)
            {
                UnsubscribeUnit(unit);
            }
            ownedUnits.Clear();
            factionA.Clear();
            factionB.Clear();
            foreach (Unit unit in unitsToRelease)
            {
                unit.Deactivate();
                Destroy(unit.gameObject);
            }

        }

        private Transform GetRootByFaction(Unit.Faction faction)
        {
            return faction == Unit.Faction.FactionA ? factionARoot : factionBRoot;
        }


        private void HandleUnitDeath(Unit deadUnit)
        {
            if (!ownedUnits.Contains(deadUnit) || !GetFactionUnits(deadUnit.faction).Remove(deadUnit))
                return;

            int batchVersion = unitBatchVersion;
            List<Unit> faction = GetFactionUnits(deadUnit.faction);
            bool factionBecameEmpty = faction.Count == 0;
            OnUnitDeath?.Invoke(deadUnit);
            if (batchVersion != unitBatchVersion)
                return;

            if (factionBecameEmpty && faction.Count == 0)
            {
                OnFactionAllDead?.Invoke(deadUnit.faction);
            }

        }

        private void HandleDamageTaken(Units.Damages.Damage damage)
        {
            OnUnitDamageTaken?.Invoke(damage);
        }


        private void HandleUnitSkillCast(Units.Unit unit, Units.Skills.Skill skill)
        {
            OnAnyUnitSkillCast?.Invoke(unit, skill);
        }


        public IReadOnlyList<Unit> GetOwnedUnits()
        {
            return ownedUnits.Where(unit => unit != null).ToArray();
        }

        public IReadOnlyList<Unit> GetAliveUnits(Unit.Faction faction)
        {
            return GetFactionUnits(faction).Where(unit => unit != null).ToArray();
        }

        public int GetAliveUnitCount(Unit.Faction faction)
        {
            return GetFactionUnits(faction).Count(unit => unit != null);
        }

        public IReadOnlyList<Unit> GetActiveUnits(Unit.Faction faction)
        {
            return GetFactionUnits(faction).Where(unit => unit != null && unit.IsActive).ToArray();
        }

        public IReadOnlyList<Unit> GetActiveUnits()
        {
            return factionA.Concat(factionB).Where(unit => unit != null && unit.IsActive).ToArray();
        }

        public List<Unit> FindActiveUnitsInRange(Unit.Faction faction, Vector2 center, float range)
        {
            float rangeSquared = range * range;
            return GetFactionUnits(faction)
                .Where(unit => unit != null && unit.IsActive)
                .Where(unit => ((Vector2)unit.transform.position - center).sqrMagnitude <= rangeSquared)
                .ToList();
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
            return FindEnemiesInRangeAtPosition(self.faction, self.transform.position, range);
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

        public Unit FindClosestEnemy(Unit self)
        {
            var enemyList = self.faction == Unit.Faction.FactionA ? factionB : factionA;
            return FindClosestUnit(enemyList, self, includeSelf: false);
        }

        public Unit FindClosestAlly(Unit self, bool includeSelf = false)
        {
            var allyList = self.faction == Unit.Faction.FactionA ? factionA : factionB;
            return FindClosestUnit(allyList, self, includeSelf);
        }

        private static Unit FindClosestUnit(IReadOnlyList<Unit> units, Unit self, bool includeSelf)
        {
            Vector2 selfPos = self.transform.position;
            float bestSqr = float.MaxValue;
            Unit closestUnit = null;

            for (int i = 0; i < units.Count; i++)
            {
                Unit candidate = units[i];
                if (candidate == null || !candidate.IsActive || (!includeSelf && candidate == self))
                {
                    continue;
                }

                float sqrDistance = ((Vector2)candidate.transform.position - selfPos).sqrMagnitude;
                if (sqrDistance < bestSqr)
                {
                    bestSqr = sqrDistance;
                    closestUnit = candidate;
                }
            }

            return closestUnit;
        }

        public List<Unit> FindEnemiesInRangeAtPosition(Unit.Faction selfFaction, Vector2 center, float range)
        {
            Unit.Faction enemyFaction = selfFaction == Unit.Faction.FactionA ? Unit.Faction.FactionB : Unit.Faction.FactionA;
            return FindActiveUnitsInRange(enemyFaction, center, range);
        }
    }
}