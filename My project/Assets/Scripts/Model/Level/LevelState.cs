using System;
using System.Collections.Generic;
using System.Linq;
using Model.Rewards;
using Model.Tetri;
using UnityEngine;

namespace Model 
{
    /// <summary>
    /// Runtime state of the current level run.
    /// The mutable progress value lives here, while the numeric tuning values are editor-adjustable parameters
    /// that shape the difficulty curve.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelState", menuName = "ScriptableObjects/LevelState", order = 1)]
    public class LevelState : ScriptableObject
    {
        private int currentLevel = 0;

        [SerializeField] private int levelsPerEnemyIncrease = 3; // 每增加一个敌人的关卡数
        [SerializeField] private int maxEnemyCount = 10; // 最大敌人数量
        [SerializeField] private int levelsPerCellIncrease = 8; // 每增加一个 TetriCell 的关卡数
        [SerializeField] private int maxAddedCellCount = 10; // 每个敌人最多的 TetriCell 数量
        [SerializeField] private Model.Tetri.CellDatabase cellDatabase;

        public int CurrentLevel => currentLevel;
        public int LevelsPerEnemyIncrease => levelsPerEnemyIncrease;
        public int MaxEnemyCount => maxEnemyCount;
        public int LevelsPerCellIncrease => levelsPerCellIncrease;
        public int MaxAddedCellCount => maxAddedCellCount;
        public Model.Tetri.CellDatabase CellDatabase => cellDatabase;

        public void AdvanceToNextLevel()
        {
            currentLevel++;
        }

        internal void Reset()
        {
            currentLevel = 0;
        }
    }
}