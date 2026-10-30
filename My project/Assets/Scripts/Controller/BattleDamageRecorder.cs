using System.Collections.Generic;
using Model.Battle.Statistics;
using Units;
using Units.Damages;
using UnityEngine;

namespace Controller
{
    public class BattleDamageRecorder : MonoBehaviour
    {
        [SerializeField] private BattleField battleField;
        [SerializeField] private UnitManager unitManager;

        private readonly DamageDatabase damageDatabase = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (battleField == null)
                Debug.LogError($"{nameof(battleField)} is not assigned.", this);
            if (unitManager == null)
                Debug.LogError($"{nameof(unitManager)} is not assigned.", this);
        }
#endif

        private void OnEnable()
        {
            battleField.OnBattleStarted += ClearRecords;
            battleField.OnBattleCleared += ClearRecords;
            unitManager.OnUnitDamageTaken += HandleDamageTaken;
        }

        private void OnDisable()
        {
            if (battleField != null)
            {
                battleField.OnBattleStarted -= ClearRecords;
                battleField.OnBattleCleared -= ClearRecords;
            }
            if (unitManager != null)
                unitManager.OnUnitDamageTaken -= HandleDamageTaken;
        }

        private void ClearRecords()
        {
            damageDatabase.Reset();
        }

        private void HandleDamageTaken(Damage damage)
        {
            if (battleField.State == BattleField.BattleState.Running)
                damageDatabase.AddDamage(damage);
        }

        public IReadOnlyList<UnitDamageSummary> GetUnitSummaries()
        {
            return damageDatabase.GetDamageByUnit();
        }
    }
}