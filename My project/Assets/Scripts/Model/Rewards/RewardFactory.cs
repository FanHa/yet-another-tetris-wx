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
        private readonly CellGroupConfig cellGroupConfig;

        public RewardFactory(TetrisResources tetrisResources, OperationTable operationTable, CellGroupConfig cellGroupConfig)
        {
            this.operationTable = operationTable;   
            this.tetrisResources = tetrisResources;
            this.cellGroupConfig = cellGroupConfig;
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
            bool characterRewardGenerated = false; // 标志是否已经生成过 CreateCharacterReward

            for (int i = 1; i <= rewardCount; i++)
            {
                // 随机决定奖励类型
                int rewardType = UnityEngine.Random.Range(0, 3); // 0: Tetri, 1: Character, 2: UpgradeTetri

                Reward reward = rewardType switch
                {
                    0 => CreateAddTetriReward(),
                    1 when !characterRewardGenerated => // 仅当未生成过 Character 奖励时调用
                        CreateCharacterReward(),
                    2 => CreateUpgradeTetriReward(),
                    _ => CreateAddTetriReward() // 如果已经生成过 Character 奖励，默认生成 AddTetri 奖励
                };

                // 如果生成的是 Character 奖励，更新标志
                if (reward is NewCharacter)
                {
                    characterRewardGenerated = true;
                }

                rewards.Add(reward);
            }
            return rewards;
        }

        private Reward CreateAddTetriReward()
        {
            List<Type> availableCellTypes;
            CellGroupConfig.Group group = CellGroupConfig.Group.None;
            // 定义普通 Tetri 和专精 Tetri 的概率
            float normalTetriProbability = 0.7f; // 普通 Tetri 的概率
            float specializedTetriProbability = 0.3f; // 专精 Tetri 的概率

            float randomValue = UnityEngine.Random.value;
            List<Type> existingCellTypes = tetrisResources.CellTypes
                .Where(type => type != typeof(Model.Tetri.Padding) && type != typeof(Model.Tetri.Empty)) // 排除 Padding 和 Empty
                .ToList();
            if (randomValue <= normalTetriProbability)
            {
                
                availableCellTypes = cellTypes
                    .Where(type => !existingCellTypes.Contains(type))
                    .ToList();
                if (availableCellTypes.Count == 0)
                {
                    // todo 不要报错,是正常游戏进行中可能会遇到的事情
                    throw new InvalidOperationException("No Tetri subclasses available for creation.");
                }

                // 随机选择一个 Tetri 类型
            }
            else if (randomValue <= normalTetriProbability + specializedTetriProbability)
            {
                group = GetRandomUnusedGroup();

                // 从 CellGroupConfig 中获取属于该 Group 的所有 CellType
                availableCellTypes = cellGroupConfig.GetCellsForGroup(group);

                if (availableCellTypes == null || availableCellTypes.Count == 0)
                {
                    throw new InvalidOperationException($"No CellTypes found for group {group} in CellGroupConfig.");
                }
            }
            else
            {
                // 默认返回 null（理论上不会到这里, todo log warn
                return null;
            }

            Type selectedCellType;
            if (availableCellTypes.Count > 0)
            {
                selectedCellType = availableCellTypes[UnityEngine.Random.Range(0, availableCellTypes.Count)];
            }
            else
            {
                // todo log
                selectedCellType = existingCellTypes[UnityEngine.Random.Range(0, existingCellTypes.Count)];
            }
                // 随机选择一个 CellType

            // 使用 Activator 创建 Cell 实例
            Cell cellTemplate = (Cell)Activator.CreateInstance(selectedCellType);

            // 创建一个 Tetri 实例
            Model.Tetri.Tetri tetriInstance = tetrisFactory.CreateRandomBaseShape();
            tetriInstance.Group = group;

            // 创建 AddTetri 奖励
            return new AddTetri(tetriInstance, cellTemplate);
        }

        private CellGroupConfig.Group GetRandomUnusedGroup()
        {
            // 获取所有已配置的 CellGroup
            var allConfiguredGroups = cellGroupConfig.Groups
                .Select(groupConfig => groupConfig.group)
                .Where(group => group != CellGroupConfig.Group.None) // 排除 None
                .ToList();

            // 获取当前 TetrisResources 中已存在的 Groups
            var existingGroups = tetrisResources.TetriGroups;

            // 找到所有不存在于 TetrisResources 中的 Groups
            var unusedGroups = allConfiguredGroups
                .Where(group => !existingGroups.Contains(group))
                .ToList();

            if (unusedGroups.Count == 0)
            {
                return CellGroupConfig.Group.None;
            }

            // 随机挑选一个未使用的 Group
            return unusedGroups[UnityEngine.Random.Range(0, unusedGroups.Count)];
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
                weights[characterType] = count > 0 ? baseWeight / (3+count) : baseWeight;
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

            // todo log 默认返回第一个（理论上不会到这里）
            return weights.Keys.First();
        }

        private Reward CreateUpgradeTetriReward()
        {
            // 从 TetrisResources 中随机挑选一个包含 PaddingCell 的 Tetri
            Tetri.Tetri targetTetri = tetrisResources.GetUnusedTetris()
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

            Model.Tetri.CellGroupConfig.Group group = targetTetri.Group;
            List<Type> availableCellTypes;

            if (group != CellGroupConfig.Group.None)
            {
                // 当 Group 不是 None 时，使用现有逻辑
                List<Type> sameGroupCellTypes = cellGroupConfig.GetCellsForGroup(group);

                var existCells = targetTetri.Shape.Cast<Model.Tetri.Cell>()
                    .Where(cell => cell != null
                        && cell is not Padding
                        && cell is not Empty)
                    .ToList();

                if (existCells.Count == 0)
                {
                    throw new InvalidOperationException("No non-Padding Cell found in the selected Tetri.");
                }

                availableCellTypes = sameGroupCellTypes
                    .Where(type => !existCells.Any(cell => cell.GetType() == type))
                    .ToList();

                if (availableCellTypes.Count == 0)
                {
                    throw new InvalidOperationException("No available cell types found in the same group that are not already in existCells.");
                }
            }
            else
            {
                // 当 Group 为 None 时，从 TetrisResources 的 cellTypes 中随机取一个
                availableCellTypes = tetrisResources.CellTypes
                    .Where(type => type != typeof(Model.Tetri.Padding) && type != typeof(Model.Tetri.Empty)) // 排除 Padding 和 Empty
                    .ToList();
                if (availableCellTypes.Count == 0)
                {
                    throw new InvalidOperationException("No available cell types found in TetrisResources.");
                }
            }

            var selectedCellType = availableCellTypes[UnityEngine.Random.Range(0, availableCellTypes.Count)];
            var newCell = (Model.Tetri.Cell)Activator.CreateInstance(selectedCellType);


            // 创建 UpgradeTetri 奖励
            return new UpgradeTetri(targetTetri, targetPosition, newCell);
        }

    }
}
