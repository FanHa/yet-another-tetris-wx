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
        private int rewardCount = 3;
        private TetrisFactory tetrisFactory = new TetrisFactory();
        private readonly TetrisResources tetrisResources;
        private readonly OperationTable operationTable;

        public RewardFactory(TetrisResources tetrisResources, OperationTable operationTable)
        {
            this.operationTable = operationTable;   
            this.tetrisResources = tetrisResources;
        }

        private static readonly List<Type> characterTypes = Assembly.GetAssembly(typeof(Cell))
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Model.Tetri.Character)) && !type.IsAbstract )
            .ToList();

        private static readonly List<Type> cellTypes = Assembly.GetAssembly(typeof(Cell))
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Cell))
                            && !type.IsSubclassOf(typeof(Character))
                            && type != typeof(Empty) 
                             && type != typeof(Padding)
                            && !type.IsAbstract)
            .ToList();
        

        public List<Reward> GenerateRewards()
        {
            List<Reward> rewards = new List<Reward>();

            for (int i = 1; i <= rewardCount; i++)
            {
                // 随机决定奖励类型
                int rewardType = UnityEngine.Random.Range(0, 3); // 0: Tetri, 1: Character, 2: UpgradeTetri

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
            List<Type> existingCellTypes = tetrisResources.CellTypes.ToList();

            List<Type> availableCellTypes = cellTypes
                .Where(type => !existingCellTypes.Contains(type))
                .ToList();
            if (availableCellTypes.Count == 0)
            {
                // todo 不要报错,是正常游戏进行中可能会遇到的事情
                throw new InvalidOperationException("No Tetri subclasses available for creation.");
            }

            // 随机选择一个 Tetri 类型
            int index = UnityEngine.Random.Range(0, availableCellTypes.Count);
            Type cellType = availableCellTypes[index];
            // 使用 Activator 创建 Cell 实例
            Cell cellTemplate = (Cell)Activator.CreateInstance(cellType);
            Model.Tetri.Tetri tetriInstance;
            if (cellTemplate is GarbageReuse)
            {
                tetriInstance = tetrisFactory.CreateCircleShape();
            }
            else
            {
                
                tetriInstance = tetrisFactory.CreateRandomBaseShape();
            }

            // 创建 AddTetri 奖励
            return new AddTetri(tetriInstance, cellTemplate);
        }

        private Reward CreateCharacterReward()
        {
            if (characterTypes.Count == 0)
            {
                throw new InvalidOperationException("No Character subclasses found.");
            }
            // 获取当前角色类型的数量
            Dictionary<Type, int> characterCounts = operationTable.GetCharacterCounts();

            // 计算权重：数量越多，权重越低
            Dictionary<Type, float> weights = new Dictionary<Type, float>();
            foreach (var characterType in characterTypes)
            {
                int count = characterCounts.ContainsKey(characterType) ? characterCounts[characterType] : 0;

                // 初始权重为 1
                float baseWeight = 1f;

                // 如果已有实例，权重减半
                weights[characterType] = count > 0 ? baseWeight / (1+count) : baseWeight;
            }

            // 根据权重随机选择角色类型
            Type selectedCharacterType = GetRandomTypeByWeight(weights);
            // 使用 Activator 创建 Character 实例
            var characterInstance = (Model.Tetri.Character)Activator.CreateInstance(selectedCharacterType);

            // 创建 NewCharacter 奖励
            return new NewCharacter(characterInstance);
        }

        private Type GetRandomTypeByWeight(Dictionary<Type, float> weights)
        {
            float totalWeight = weights.Values.Sum();
            float randomValue = UnityEngine.Random.Range(0, totalWeight);

            float cumulativeWeight = 0f;
            foreach (var kvp in weights)
            {
                cumulativeWeight += kvp.Value;
                if (randomValue <= cumulativeWeight)
                {
                    return kvp.Key;
                }
            }

            // 默认返回第一个（理论上不会到这里）
            return weights.Keys.First();
        }
        private Reward CreateUpgradeTetriReward()
        {
            var availableCellTypes = tetrisResources.CellTypes
                .Where(type => type != typeof(Model.Tetri.Padding)) // 排除 Padding 类型
                .ToList();
            if (availableCellTypes.Count == 0)
            {
                throw new InvalidOperationException("No usable Cell types found in TetrisResources.");
            }

            // 随机选择一个 Cell 类型
            int cellTypeIndex = UnityEngine.Random.Range(0, availableCellTypes.Count);
            Type cellType = availableCellTypes[cellTypeIndex];

            // 从 TetrisResources 中随机挑选一个包含 PaddingCell 的 Tetri
            var targetTetri = tetrisResources.GetUnusedTetris()
                .Concat(tetrisResources.GetUsedTetris())
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

            int positionIndex = UnityEngine.Random.Range(0, paddingPositions.Count);
            Vector2Int targetPosition = paddingPositions[positionIndex];

            // 创建用于升级的 Cell 实例
            var newCell = (Model.Tetri.Cell)Activator.CreateInstance(cellType);

            // 创建 UpgradeTetri 奖励
            return new UpgradeTetri(targetTetri, targetPosition, newCell);
        }

    }
}
