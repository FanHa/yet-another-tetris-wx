using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Model.Rewards;
using Model.Tetri;
using UnityEngine;

namespace Model 
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public int currentLevel = 0;
        public List<EnemyData> enemyDatas; // 敌人数据列表

        [SerializeField] private int levelsPerEnemyIncrease = 3; // 每增加一个敌人的关卡数
        [SerializeField] private int maxEnemyCount = 10; // 最大敌人数量
        [SerializeField] private int levelsPerCellIncrease = 8; // 每增加一个 TetriCell 的关卡数
        [SerializeField] private int maxAddedCellCount = 10; // 每个敌人最多的 TetriCell 数量
        [SerializeField] private Model.Tetri.TetriCellFactory tetriCellModelFactory; // TetriCell 工厂

        private List<CellTypeId> availableCellTypeIds;
        private List<CharacterTypeId> availableCharacterTypeIds;
        
        private void OnEnable()
        {
            availableCellTypeIds = Enum.GetValues(typeof(CellTypeId)).Cast<CellTypeId>().ToList();
            availableCharacterTypeIds = Enum.GetValues(typeof(CharacterTypeId)).Cast<CharacterTypeId>().ToList();
        }

        /// <summary>
        /// 主动增加关卡并生成敌人数据
        /// </summary>
        public void AdvanceToNextLevel()
        {
            currentLevel++; // 增加关卡数
            GenerateEnemyData(); // 生成新的敌人数据
        }

        private void GenerateEnemyData()
        {
            // 确保敌人数据列表不为空
            if (enemyDatas == null)
            {
                enemyDatas = new List<EnemyData>();
            }

            // 计算当前关卡的敌人数量
            int enemyCount = Mathf.Min(1 + (currentLevel - 1) / levelsPerEnemyIncrease, maxEnemyCount);

            // 确保敌人数量足够
            while (enemyDatas.Count < enemyCount)
            {
                // 添加一个随机敌人
                var randomCharacterId = availableCharacterTypeIds[UnityEngine.Random.Range(0, availableCharacterTypeIds.Count)];
                enemyDatas.Add(new EnemyData
                {
                    characterId = randomCharacterId,
                    tetriCellIds = new List<CellTypeId>()
                });
            }

            // 如果是第 0 关，直接返回，不增加 TetriCells
            if (currentLevel == 0)
            {
                return;
            }

            // 为随机敌人增加 TetriCells
            int additionalCells = Mathf.Min((currentLevel - 1) / levelsPerCellIncrease + 1, maxAddedCellCount); // 每10关增加一个随机 TetriCell，最多10个
            for (int i = 0; i < additionalCells; i++)
            {
                // 随机选择一个敌人
                int randomEnemyIndex = UnityEngine.Random.Range(0, enemyDatas.Count);
                var randomEnemy = enemyDatas[randomEnemyIndex];

                // 随机添加一个 TetriCell
                var randomCellId = availableCellTypeIds[UnityEngine.Random.Range(0, availableCellTypeIds.Count)];
                randomEnemy.tetriCellIds.Add(randomCellId);
            }
        }

        /// <summary>
        /// 获取当前关卡的敌人数据
        /// </summary>
        public List<InventoryItem> GetEnemyData()
        {
            // 转换为 InventoryItem 列表
            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            foreach (var enemy in enemyDatas)
            {

                var characterInstance = (Tetri.Character)tetriCellModelFactory.CreateCharacterCell(enemy.characterId);

                var tetriCells = new List<Tetri.Cell>();
                foreach (var cellId in enemy.tetriCellIds)
                {
                    tetriCells.Add(tetriCellModelFactory.CreateCell(cellId));
                }

                InventoryItem inventoryItem = new InventoryItem(characterInstance, tetriCells);
                inventoryItems.Add(inventoryItem);
            }

            return inventoryItems;
        }

        internal void Reset()
        {
            // 重置当前关卡数为 0
            currentLevel = 0;

            // 清空敌人数据
            if (enemyDatas != null)
            {
                enemyDatas.Clear();
            }

            // 重新生成关卡 0 的敌人数据
            GenerateEnemyData();
        }

        [Serializable]
        public struct EnemyData {
            public CharacterTypeId characterId; // 敌人名称
            public List<CellTypeId> tetriCellIds; // TetriCell 列表
        }
    }
}