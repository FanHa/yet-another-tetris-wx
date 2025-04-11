using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Controller;
using Model.Tetri;

namespace Model.Rewards
{
    public class RewardFactory
    {
        private readonly Random random = new Random();
        private int rewardCount = 3;
        private TetrisFactory tetrisFactory = new TetrisFactory();

        private static readonly List<Type> characterTypes = Assembly.GetAssembly(typeof(Cell))
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Model.Tetri.Character)) && !type.IsAbstract )
            .ToList();

        private static readonly List<Type> cellTypes = Assembly.GetAssembly(typeof(Cell))
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Cell)) && !type.IsSubclassOf(typeof(Character)) && !type.IsAbstract)
            .ToList();

        public List<Reward> GenerateRewards()
        {
            List<Reward> rewards = new List<Reward>();

            for (int i = 1; i <= rewardCount; i++)
            {
                Reward reward = CreateRandomReward();
                rewards.Add(reward);
                
            }

            return rewards;
        }

        private Reward CreateRandomReward()
        {
            // 随机决定生成哪种奖励类型
            bool isTetriReward = random.Next(2) == 0;

            if (isTetriReward)
            {
                return CreateTetriReward();
            }
            else
            {
                return CreateCharacterReward();
            }

        }

        private Reward CreateTetriReward()
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
    }
}
