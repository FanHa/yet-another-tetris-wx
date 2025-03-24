using System.Collections.Generic;
using UnityEngine;

namespace Model 
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public string levelName; // 关卡名称
        public List<InventoryItem> enemyData; // 敌人数据列表

        public List<InventoryItem> GetEnemyData()
        {
            return enemyData;
        }
    }
}