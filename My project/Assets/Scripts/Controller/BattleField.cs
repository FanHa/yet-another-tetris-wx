using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Units;
using System.Linq;
using Model;

namespace Controller {
    public interface IBattleEffect
    {
        event Action<IBattleEffect> Deactivated;
        void Deactivate();
    }

    [RequireComponent(typeof(UnitManager))]
    public class BattleField : MonoBehaviour
    {
        [Header("Spawn Points")]
        // public Transform spawnPointA;
        public Transform spawnPointB;
        [SerializeField] private float timeDelayBeforeBattle; // 战斗开始前的延迟时间
        private Coroutine spawnRoutine; // 处理战斗开始生成单位后需要延迟一小段时间再开打

        [Header("Data")]
        private UnitManager unitManager;
        public event Action OnBattleStarted;
        public event Action<Unit.Faction> OnBattleResult;
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
        private readonly HashSet<IBattleEffect> battleEffects = new();

        void Awake()
        {
            unitManager = GetComponent<UnitManager>();
        }
        void Start()
        {
            unitManager.OnUnitDeath += HandleUnitDeath;
            unitManager.OnFactionAllDead += HandleFactionAllDead;
            unitManager.OnUnitClicked += HandleUnitClicked;
        }

        private void HandleUnitClicked(Unit unit)
        {
            OnUnitClicked?.Invoke(unit);
        }

        public void PrepareBattle(
            IReadOnlyList<CharacterPlacement> factionAItems,
            IReadOnlyList<CharacterPlacement> factionBItems)
        {
            if (State != BattleState.Empty)
                throw new InvalidOperationException("The battlefield must be empty before a battle can be prepared.");

            State = BattleState.Prepared;
            unitManager.SpawnUnits(
                factionAItems,
                transform,
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
            spawnRoutine = StartCoroutine(DelayActivateUnitsCoroutine());
        }

        private void CancelPendingActivation()
        {
            if (spawnRoutine == null)
            {
                return;
            }

            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        public void RegisterEffect(IBattleEffect effect)
        {
            if (State != BattleState.Running)
                throw new InvalidOperationException("Battle effects can only be registered while the battle is running.");

            if (battleEffects.Add(effect))
                effect.Deactivated += HandleEffectDeactivated;
        }

        public void UnregisterEffect(IBattleEffect effect)
        {
            if (battleEffects.Remove(effect))
                effect.Deactivated -= HandleEffectDeactivated;
        }

        private void HandleEffectDeactivated(IBattleEffect effect)
        {
            UnregisterEffect(effect);
        }

        private void ClearEffects()
        {
            IBattleEffect[] effects = battleEffects.ToArray();
            foreach (IBattleEffect effect in effects)
                effect.Deactivated -= HandleEffectDeactivated;

            battleEffects.Clear();
            foreach (IBattleEffect effect in effects)
            {
                effect.Deactivate();
            }
        }

        private void FinishBattle(Unit.Faction defeatedFaction)
        {
            State = BattleState.Finished;
            ClearEffects();
            unitManager.DeactivateAllUnits();
            OnBattleResult?.Invoke(defeatedFaction);
        }

        public void ClearBattle()
        {
            bool hadBattle = State != BattleState.Empty;
            CancelPendingActivation();
            State = BattleState.Empty;
            ClearEffects();
            unitManager.ReleaseAllUnits();

            if (hadBattle)
                OnBattleCleared?.Invoke();
        }

        private IEnumerator DelayActivateUnitsCoroutine()
        {
            yield return new WaitForSeconds(timeDelayBeforeBattle);
            if (State != BattleState.Starting)
                yield break;

            spawnRoutine = null;
            State = BattleState.Running;
            OnBattleStarted?.Invoke();
            unitManager.ActivateAllUnits();
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

            FinishBattle(faction);
        }

        private void OnDestroy()
        {
            State = BattleState.Empty;
            CancelPendingActivation();
            if (unitManager != null)
            {
                unitManager.OnUnitDeath -= HandleUnitDeath;
                unitManager.OnFactionAllDead -= HandleFactionAllDead;
                unitManager.OnUnitClicked -= HandleUnitClicked;
            }
            ClearEffects();
        }
        
    }
}
