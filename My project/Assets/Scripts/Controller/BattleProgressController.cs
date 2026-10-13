using Units;
using UnityEngine;

namespace Controller
{
    public class BattleProgressController : MonoBehaviour
    {
        [SerializeField] private BattleField battleField;
        [SerializeField] private GameStatusHUD gameStatusHud;

        private bool progressEnabled;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (battleField == null)
                Debug.LogError($"{nameof(battleField)} is not assigned.", this);
            if (gameStatusHud == null)
                Debug.LogError($"{nameof(gameStatusHud)} is not assigned.", this);
        }
#endif

        private void OnEnable()
        {
            battleField.OnUnitDeath += HandleUnitDeath;
            battleField.OnBattleFinished += HandleBattleFinished;
            battleField.OnBattleCleared += HandleBattleCleared;
        }

        private void OnDisable()
        {
            if (battleField != null)
            {
                battleField.OnUnitDeath -= HandleUnitDeath;
                battleField.OnBattleFinished -= HandleBattleFinished;
                battleField.OnBattleCleared -= HandleBattleCleared;
            }
            progressEnabled = false;
        }

        public void SetProgressEnabled(bool enabled)
        {
            progressEnabled = enabled;
        }

        public void SetLevel(int level)
        {
            gameStatusHud.SetLevel(level);
        }

        private void HandleUnitDeath(Unit unit)
        {
            if (progressEnabled && unit.faction == Unit.Faction.FactionB)
                gameStatusHud.AddScore(1);
        }

        private void HandleBattleFinished(BattleResult result)
        {
            bool applyProgress = progressEnabled;
            progressEnabled = false;
            if (applyProgress && result.DefeatedFaction == Unit.Faction.FactionA)
                gameStatusHud.DecreaseLife(1);
        }

        private void HandleBattleCleared()
        {
            progressEnabled = false;
        }
    }
}