using System;
using System.Collections;
using Units;
using UI;
using UnityEngine;

namespace Controller
{
    public class BattleSettlementController : MonoBehaviour
    {
        [SerializeField] private BattleField battleField;
        [SerializeField] private BattleDamageRecorder damageRecorder;
        [SerializeField] private BattleStatistics battleStatistics;
        [SerializeField] private float displayDelay = 2f;

        public event Action OnSettlementConfirmed;

        private Coroutine displayRoutine;
        private int displayVersion;
        private bool isShowing;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (battleField == null)
                Debug.LogError($"{nameof(battleField)} is not assigned.", this);
            if (damageRecorder == null)
                Debug.LogError($"{nameof(damageRecorder)} is not assigned.", this);
            if (battleStatistics == null)
                Debug.LogError($"{nameof(battleStatistics)} is not assigned.", this);
            if (displayDelay < 0f || float.IsNaN(displayDelay) || float.IsInfinity(displayDelay))
                Debug.LogError($"{nameof(displayDelay)} must be finite and non-negative.", this);
            if (battleStatistics != null && (battleStatistics.transform == transform || transform.IsChildOf(battleStatistics.transform)))
                Debug.LogError("The settlement controller must remain active when the statistics panel is hidden.", this);
        }
#endif

        private void OnEnable()
        {
            battleField.OnBattleResult += HandleBattleResult;
            battleField.OnBattleCleared += CancelDisplay;
            battleStatistics.OnEndStatistics += HandleConfirmation;
            CancelDisplay();
        }

        private void OnDisable()
        {
            if (battleField != null)
            {
                battleField.OnBattleResult -= HandleBattleResult;
                battleField.OnBattleCleared -= CancelDisplay;
            }
            if (battleStatistics != null)
                battleStatistics.OnEndStatistics -= HandleConfirmation;
            CancelDisplay();
        }

        private void HandleBattleResult(Unit.Faction defeatedFaction)
        {
            CancelDisplay();
            displayRoutine = StartCoroutine(DisplayStatistics(displayVersion));
        }

        private IEnumerator DisplayStatistics(int version)
        {
            yield return new WaitForSeconds(displayDelay);
            if (version != displayVersion || battleField.State != BattleField.BattleState.Finished)
                yield break;

            displayRoutine = null;
            isShowing = true;
            battleStatistics.ShowStatistics(damageRecorder.GetUnitSummaries(), Unit.Faction.FactionA);
            battleStatistics.gameObject.SetActive(true);
        }

        private void CancelDisplay()
        {
            displayVersion++;
            isShowing = false;
            if (displayRoutine != null)
            {
                StopCoroutine(displayRoutine);
                displayRoutine = null;
            }
            if (battleStatistics != null)
                battleStatistics.gameObject.SetActive(false);
        }

        private void HandleConfirmation()
        {
            if (!isShowing || battleField.State != BattleField.BattleState.Finished)
                return;

            isShowing = false;
            battleField.ClearBattle();
            OnSettlementConfirmed?.Invoke();
        }
    }
}