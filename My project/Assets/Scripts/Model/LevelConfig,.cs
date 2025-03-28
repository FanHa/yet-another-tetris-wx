using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Model.Rewards;
using UnityEngine;

namespace Model 
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public string levelName; // 关卡名称
        public List<EnemyData> enemyData; // 敌人数据列表

        public List<InventoryItem> GetEnemyData()
        {
            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            foreach (var enemy in enemyData)
            {

                var characterInstance = (Tetri.Character)Activator.CreateInstance(enemy.character.Type);


                var tetriCells = new List<Tetri.Cell>();
                foreach (var cell in enemy.tetriCells)
                {
                    tetriCells.Add((Tetri.Cell)Activator.CreateInstance(cell.Type));
                }
                InventoryItem inventoryItem = new InventoryItem(characterInstance, enemy.spawnInterval, tetriCells);
                inventoryItems.Add(inventoryItem);
            }
            return inventoryItems;
        }

        [Serializable]
        public struct EnemyData {
            public CharacterTypeReference character; // 敌人名称
            public int spawnInterval; // 出生间隔
            public List<CellTypeReference> tetriCells; // TetriCell 列表
        }
    }
}