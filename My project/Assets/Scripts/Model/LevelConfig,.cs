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
        [SerializeField] private List<CharacterTypeReference> availableCharacters; // 可用的角色类型
        [SerializeField] private List<CellTypeReference> availableTetriCells; // 可用的 TetriCell 类型
        
        [SerializeField] private int levelsPerEnemyIncrease = 3; // 每增加一个敌人的关卡数
        [SerializeField] private int maxEnemyCount = 10; // 最大敌人数量
        [SerializeField] private int levelsPerCellIncrease = 8; // 每增加一个 TetriCell 的关卡数
        [SerializeField] private int maxAddedCellCount = 10; // 每个敌人最多的 TetriCell 数量
        [SerializeField] private Model.Tetri.TetriCellFactory tetriCellModelFactory; // TetriCell 工厂

        
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
                var randomCharacter = availableCharacters[UnityEngine.Random.Range(0, availableCharacters.Count)];
                var tetriCells = new List<CellTypeReference>();
                // tetriCells.Add(randomCharacter);
                // EnemyData enemyData = new EnemyData
                // {
                //     character = randomCharacter,
                //     tetriCells = new List<CellTypeReference>()
                // };

                enemyDatas.Add(new EnemyData
                {
                    character = randomCharacter,
                    tetriCells = new List<CellTypeReference>()
                    
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
            foreach (var enemy in enemyDatas)
            {

                var characterInstance = (Tetri.Character)tetriCellModelFactory.CreateCell(enemy.character.Type);

                var tetriCells = new List<Tetri.Cell>();
                foreach (var cell in enemy.tetriCells)
                {
                    tetriCells.Add(tetriCellModelFactory.CreateCell(cell.Type));
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
            public CharacterTypeReference character; // 敌人名称
            public List<CellTypeReference> tetriCells; // TetriCell 列表
        }
    }
}