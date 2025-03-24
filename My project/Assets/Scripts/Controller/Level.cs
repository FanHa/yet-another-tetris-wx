using UnityEngine;

namespace Controller
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private Model.LevelConfig[] levels; // 所有关卡配置
        private int currentLevelIndex = 0;

        public Model.LevelConfig GetCurrentLevelConfig()
        {
            if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length)
            {
                Debug.LogError("Invalid current level index: " + currentLevelIndex);
                return null;
            }

            return levels[currentLevelIndex];
        }

        public void AdvanceToNextLevel()
        {
            if (currentLevelIndex + 1 < levels.Length)
            {
                currentLevelIndex++;
                Debug.Log("Advanced to next level: " + levels[currentLevelIndex].levelName);
            }
            else
            {
                Debug.LogWarning("No more levels to advance to.");
            }
        }
    }
}