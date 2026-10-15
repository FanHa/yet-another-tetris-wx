using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Units;
using Model;

namespace Controller {
    public readonly struct BattleResult
    {
        public Unit.Faction DefeatedFaction { get; }

        public BattleResult(Unit.Faction defeatedFaction)
        {
            DefeatedFaction = defeatedFaction;
        }
    }

    [RequireComponent(typeof(UnitManager))]
    public class BattleField : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform spawnPointA;
        [SerializeField] private Transform spawnPointB;
        [SerializeField] private float timeDelayBeforeBattle; // 战斗开始前的延迟时间
        private Coroutine battleStartDelayCoroutine;

        [Header("Data")]
        private UnitManager unitManager;
        public event Action OnBattleStarted;
        public event Action<BattleResult> OnBattleFinished;
        public event Action OnBattleCleared;
        public event Action<Unit> OnUnitClicked;
        public event Action<Unit> OnUnitDeath;

        public enum BattleState
        {
            Empty,
            Prepared,
            Starting,
            Running,
            Finished
        }

        public BattleState State { get; private set; }

        void Awake()
        {
            unitManager = GetComponent<UnitManager>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spawnPointA == null)
                Debug.LogError($"{nameof(spawnPointA)} is not assigned.", this);
            if (spawnPointB == null)
                Debug.LogError($"{nameof(spawnPointB)} is not assigned.", this);
        }
#endif

        private void OnEnable()
        {
            unitManager.OnUnitDeath += HandleUnitDeath;
            unitManager.OnFactionAllDead += HandleFactionAllDead;
            unitManager.OnUnitClicked += HandleUnitClicked;
        }

        private void OnDisable()
        {
            if (unitManager == null)
                return;

            unitManager.OnUnitDeath -= HandleUnitDeath;
            unitManager.OnFactionAllDead -= HandleFactionAllDead;
            unitManager.OnUnitClicked -= HandleUnitClicked;
        }

        private void HandleUnitClicked(Unit unit)
        {
            OnUnitClicked?.Invoke(unit);
        }

        // Faction A's spawn point maps to the operation table center.
        public void PrepareBattle(
            IReadOnlyList<CharacterPlacement> factionAItems,
            IReadOnlyList<CharacterPlacement> factionBItems)
        {
            if (State != BattleState.Empty)
                throw new InvalidOperationException("The battlefield must be empty before a battle can be prepared.");

            State = BattleState.Prepared;
            unitManager.SpawnUnits(
                factionAItems,
                spawnPointA,
                Unit.Faction.FactionA
            );
            unitManager.SpawnUnits(
                factionBItems,
                spawnPointB,
                Unit.Faction.FactionB
            );
        }

        public void StartBattle()
        {
            if (State != BattleState.Prepared)
                throw new InvalidOperationException("The battle must be prepared before it can start.");

            State = BattleState.Starting;
            CancelPendingActivation();
            battleStartDelayCoroutine = StartCoroutine(DelayActivateUnitsCoroutine());
        }

        private void CancelPendingActivation()
        {
            if (battleStartDelayCoroutine == null)
            {
                return;
            }

            StopCoroutine(battleStartDelayCoroutine);
            battleStartDelayCoroutine = null;
        }

        private void EndBattle(Unit.Faction defeatedFaction)
        {
            State = BattleState.Finished;
            unitManager.DeactivateAllUnits();
            OnBattleFinished?.Invoke(new BattleResult(defeatedFaction));
        }

        public void ClearField()
        {
            CancelPendingActivation();
            unitManager.ReleaseAllUnits();
            State = BattleState.Empty;
            OnBattleCleared?.Invoke();
        }

        private IEnumerator DelayActivateUnitsCoroutine()
        {
            yield return new WaitForSeconds(timeDelayBeforeBattle);
            if (State != BattleState.Starting)
                yield break;

            battleStartDelayCoroutine = null;
            State = BattleState.Running;
            unitManager.ActivateAllUnits();
            OnBattleStarted?.Invoke();
        }

        private void HandleUnitDeath(Unit deadUnit)
        {
            if (State != BattleState.Running)
                return;

            OnUnitDeath?.Invoke(deadUnit);
        }

        private void HandleFactionAllDead(Unit.Faction faction)
        {
            if (State != BattleState.Running)
                return;

            if (unitManager.GetAliveUnitCount(faction) != 0)
                return;

            EndBattle(faction);
        }

        private void OnDestroy()
        {
            State = BattleState.Empty;
            CancelPendingActivation();
        }
        
    }
}
