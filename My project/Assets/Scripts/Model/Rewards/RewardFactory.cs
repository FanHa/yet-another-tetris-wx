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

        private static readonly List<Type> characterRewardTypes = Assembly.GetAssembly(typeof(Reward))
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Reward)) && !type.IsAbstract && type.IsSubclassOf(typeof(Character)))
            .ToList();

        private static readonly List<Type> tetriRewardTypes = Assembly.GetAssembly(typeof(Reward))
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Reward)) && !type.IsAbstract && type.IsSubclassOf(typeof(Tetri)))
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
            var allRewardTypes = characterRewardTypes.Concat(tetriRewardTypes).ToList();

            if (allRewardTypes.Count == 0)
            {
                throw new InvalidOperationException("No subclasses of Reward found.");
            }

            // 随机选择一个子类并创建实例
            int index = random.Next(allRewardTypes.Count);
            Reward reward = (Reward)Activator.CreateInstance(allRewardTypes[index]);
            if (reward is Tetri tetriReward)
            {
                tetriReward.SetTetri(tetrisFactory.CreateRandomShape());
                tetriReward.FillCells();
            }
            return reward;
        }
    }
}
