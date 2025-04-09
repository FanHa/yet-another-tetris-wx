using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Model.Rewards;
using UnityEngine;

namespace Model 
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public int currentLevel = 0;
        public List<EnemyData> enemyData; // 敌人数据列表
        [SerializeField] private List<CharacterTypeReference> availableCharacters; // 可用的角色类型
        [SerializeField] private List<CellTypeReference> availableTetriCells; // 可用的 TetriCell 类型
        private void OnEnable()
        {
            // 初始化 availableCharacters
            if (availableCharacters == null || availableCharacters.Count == 0)
            {
                availableCharacters = new List<CharacterTypeReference>();
                var characterTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsSubclassOf(typeof(Tetri.Character)) && !type.IsAbstract)
                    .ToList();

                foreach (var characterType in characterTypes)
                {
                    availableCharacters.Add(new CharacterTypeReference { typeName = characterType.AssemblyQualifiedName });
                }
            }

            // 初始化 availableTetriCells
            if (availableTetriCells == null || availableTetriCells.Count == 0)
            {
                availableTetriCells = new List<CellTypeReference>();
                var cellTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsSubclassOf(typeof(Tetri.Cell)) && !type.IsAbstract)
                    .ToList();

                foreach (var cellType in cellTypes)
                {
                    availableTetriCells.Add(new CellTypeReference { typeName = cellType.AssemblyQualifiedName });
                }
            }
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
            if (enemyData == null)
            {
                enemyData = new List<EnemyData>();
            }

            // 计算当前关卡的敌人数量
            int enemyCount = Mathf.Min(1 + (currentLevel - 1) / 3, 10); // 每3关增加一个敌人，最多10个

            // 确保敌人数量足够
            while (enemyData.Count < enemyCount)
            {
                // 添加一个随机敌人
                var randomCharacter = availableCharacters[UnityEngine.Random.Range(0, availableCharacters.Count)];
                enemyData.Add(new EnemyData
                {
                    character = randomCharacter,
                    tetriCells = new List<CellTypeReference>()
                });
            }

            // 为随机敌人增加 TetriCells
            int additionalCells = Mathf.Min((currentLevel - 1) / 10 + 1, 10); // 每10关增加一个随机 TetriCell，最多10个
            for (int i = 0; i < additionalCells; i++)
            {
                // 随机选择一个敌人
                int randomEnemyIndex = UnityEngine.Random.Range(0, enemyData.Count);
                var randomEnemy = enemyData[randomEnemyIndex];

                // 随机添加一个 TetriCell
                var randomCell = availableTetriCells[UnityEngine.Random.Range(0, availableTetriCells.Count)];
                randomEnemy.tetriCells.Add(randomCell);
            }
        }

        /// <summary>
        /// 获取当前关卡的敌人数据
        /// </summary>
        public List<InventoryItem> GetEnemyData()
        {
            // 转换为 InventoryItem 列表
            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            foreach (var enemy in enemyData)
            {
                var characterInstance = (Tetri.Character)Activator.CreateInstance(enemy.character.Type);

                var tetriCells = new List<Tetri.Cell>();
                foreach (var cell in enemy.tetriCells)
                {
                    tetriCells.Add((Tetri.Cell)Activator.CreateInstance(cell.Type));
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
            if (enemyData != null)
            {
                enemyData.Clear();
            }

            // 重新生成关卡 0 的敌人数据
            GenerateEnemyData();
        }

        [Serializable]
        public struct EnemyData {
            public CharacterTypeReference character; // 敌人名称
            public List<CellTypeReference> tetriCells; // TetriCell 列表
        }
    }
}