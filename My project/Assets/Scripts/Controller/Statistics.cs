using TMPro;
using UnityEngine;

namespace Controller
{
    public class Statistics : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI levelText; // 显示关卡
        [SerializeField] private TextMeshProUGUI scoreText; // 显示分数
        [SerializeField] private TextMeshProUGUI lifeText;  // 显示剩余生命值

        private int currentLevel = 1; // 当前关卡
        private int currentScore = 0; // 当前分数
        private int currentLife = 3;  // 剩余生命值

        void Start()
        {
            // 初始化UI文本
            if (levelText != null)
            {
                levelText.text = $"Level: {currentLevel}";
            }

            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore}";
            }

            if (lifeText != null)
            {
                lifeText.text = $"Life: {currentLife}";
            }
        }

        /// <summary>
        /// 设置当前关卡并更新UI
        /// </summary>
        /// <param name="level">新的关卡值</param>
        public void SetLevel(int level)
        {
            currentLevel = level;
            if (levelText != null)
            {
                levelText.text = $"Level: {currentLevel}";
            }
        }

        /// <summary>
        /// 增加分数并更新UI
        /// </summary>
        /// <param name="score">增加的分数值</param>
        public void AddScore(int score)
        {
            currentScore += score;
            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore}";
            }
        }

        /// <summary>
        /// 减少生命值并更新UI
        /// </summary>
        /// <param name="amount">减少的生命值</param>
        public void DecreaseLife(int amount)
        {
            currentLife -= amount;
            if (currentLife < 0) currentLife = 0; // 防止生命值小于0
            if (lifeText != null)
            {
                lifeText.text = $"Life: {currentLife}";
            }
        }

        /// <summary>
        /// 增加生命值并更新UI
        /// </summary>
        /// <param name="amount">增加的生命值</param>
        public void IncreaseLife(int amount)
        {
            currentLife += amount;
            if (lifeText != null)
            {
                lifeText.text = $"Life: {currentLife}";
            }
        }

        /// <summary>
        /// 获取当前分数
        /// </summary>
        public int GetScore()
        {
            return currentScore;
        }

        /// <summary>
        /// 获取当前生命值
        /// </summary>
        public int GetLife()
        {
            return currentLife;
        }

        /// <summary>
        /// 获取当前关卡
        /// </summary>
        public int GetLevel()
        {
            return currentLevel;
        }
    }
}