using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Controller;
using Model.Tetri;
using UnityEngine;

namespace Model.Rewards
{
    public class RewardFactory
    {
        private readonly System.Random random = new System.Random();
        private int rewardCount = 3;
        private TetrisFactory tetrisFactory = new TetrisFactory();
        private readonly TetrisResources tetrisResources;

        public RewardFactory(TetrisResources tetrisResources)
        {
            this.tetrisResources = tetrisResources;
        }

        private static readonly List<Type> characterTypes = Assembly.GetAssembly(typeof(Cell))
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Model.Tetri.Character)) && !type.IsAbstract )
            .ToList();

        private static readonly List<Type> cellTypes = Assembly.GetAssembly(typeof(Cell))
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Cell)) && !type.IsSubclassOf(typeof(Character)) && !type.IsAbstract)
            .ToList();
        
        // todo upgradeCellTypes

        public List<Reward> GenerateRewards()
        {
            List<Reward> rewards = new List<Reward>();

            for (int i = 1; i <= rewardCount; i++)
            {
                // 随机决定奖励类型
                int rewardType = random.Next(3); // 0: Tetri, 1: Character, 2: UpgradeTetri

                Reward reward = rewardType switch
                {
                    0 => CreateAddTetriReward(),
                    1 => CreateCharacterReward(),
                    2 => CreateUpgradeTetriReward(),
                    _ => throw new InvalidOperationException("Invalid reward type.")
                };

                rewards.Add(reward);
            }

            return rewards;
        }

        private Reward CreateAddTetriReward()
        {
            if (cellTypes.Count == 0)
            {
                throw new InvalidOperationException("No Tetri subclasses found.");
            }

            // 随机选择一个 Tetri 类型
            int index = random.Next(cellTypes.Count);
            Type cellType = cellTypes[index];

            // 创建一个新的 Tetri 实例
            var tetriInstance = tetrisFactory.CreateRandomShape();

            // 使用 Activator 创建 Cell 实例
            var cellTemplate = (Cell)Activator.CreateInstance(cellType);

            // 创建 AddTetri 奖励
            return new AddTetri(tetriInstance, cellTemplate);
        }

        private Reward CreateCharacterReward()
        {
            if (characterTypes.Count == 0)
            {
                throw new InvalidOperationException("No Character subclasses found.");
            }

            // 随机选择一个 Character 类型
            int index = random.Next(characterTypes.Count);
            Type characterType = characterTypes[index];

            // 使用 Activator 创建 Character 实例
            var characterInstance = (Model.Tetri.Character)Activator.CreateInstance(characterType);

            // 创建 NewCharacter 奖励
            return new NewCharacter(characterInstance);
        }
        private Reward CreateUpgradeTetriReward()
        {
            var availableCellTypes = tetrisResources.CellTypes.ToList();
            if (availableCellTypes.Count == 0)
            {
                throw new InvalidOperationException("No usable Cell types found in TetrisResources.");
            }

            // 随机选择一个 Cell 类型
            int cellTypeIndex = random.Next(availableCellTypes.Count);
            Type cellType = availableCellTypes[cellTypeIndex];

            // 从 TetrisResources 中随机挑选一个包含 PaddingCell 的 Tetri
            var targetTetri = tetrisResources.GetUnusedTetris()
                .Where(tetri => tetri.Shape.Cast<Model.Tetri.Cell>().Any(cell => cell is Model.Tetri.Padding))
                .OrderBy(_ => Guid.NewGuid())
                .FirstOrDefault();

            if (targetTetri == null)
            {
                throw new InvalidOperationException("No Tetri with PaddingCell found in TetrisResources.");
            }

            // 随机选择一个 PaddingCell 的位置
            var paddingPositions = targetTetri.GetOccupiedPositions()
                .Where(pos => targetTetri.Shape[pos.x, pos.y] is Model.Tetri.Padding)
                .ToList();

            if (paddingPositions.Count == 0)
            {
                throw new InvalidOperationException("No PaddingCell found in the selected Tetri.");
            }

            int positionIndex = random.Next(paddingPositions.Count);
            Vector2Int targetPosition = paddingPositions[positionIndex];

            // 创建用于升级的 Cell 实例
            var newCell = (Model.Tetri.Cell)Activator.CreateInstance(cellType);

            // 创建 UpgradeTetri 奖励
            return new UpgradeTetri(targetTetri, targetPosition, newCell);
        }

    }
}
