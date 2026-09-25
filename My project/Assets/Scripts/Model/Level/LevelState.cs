using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "LevelState", menuName = "Game Data/Gameplay/Level/Level State")]
    public sealed class LevelState : ScriptableObject
    {
        [SerializeField] private int levelsPerEnemyIncrease = 3;
        [SerializeField] private int maxEnemyCount = 10;
        [SerializeField] private int levelsPerCellIncrease = 8;
        [SerializeField] private int maxAddedCellCount = 10;

        [System.NonSerialized]
        private int currentLevel;

        public int LevelsPerEnemyIncrease => levelsPerEnemyIncrease;
        public int MaxEnemyCount => maxEnemyCount;
        public int LevelsPerCellIncrease => levelsPerCellIncrease;
        public int MaxAddedCellCount => maxAddedCellCount;
        public int CurrentLevel => currentLevel;

        public void AdvanceToNextLevel()
        {
            currentLevel++;
        }

        public void Reset()
        {
            currentLevel = 0;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (levelsPerEnemyIncrease < 1 || maxEnemyCount < 1 || levelsPerCellIncrease < 1 || maxAddedCellCount < 1)
            {
                Debug.LogError($"[{nameof(LevelState)}] Level configuration values must be at least 1.", this);
            }
        }
#endif
    }
}