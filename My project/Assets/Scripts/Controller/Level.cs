using UnityEngine;

namespace Controller
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private Model.LevelConfig[] levels; // 所有关卡配置
        [SerializeField] private Statistics statisticsController; // 统计控制器
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
            currentLevelIndex++;
            if (currentLevelIndex < levels.Length)
            {
                statisticsController.SetLevel(currentLevelIndex + 1); // 更新UI
                Debug.Log("Level increased to: " + (currentLevelIndex + 1));
            }
            else
            {
                Debug.LogWarning("No more levels available.");
            }
        }
    }
}