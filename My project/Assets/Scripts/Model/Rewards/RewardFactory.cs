using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Tetri;

namespace Model.Reward
{
    public class RewardFactory
    {
        private readonly Random random = new Random();
        private int rewardCount = 3;

        public List<Item> GenerateRewards()
        {
            List<Item> rewards = new List<Item>();

            for (int i = 1; i <= rewardCount; i++)
            {
                Reward reward = CreateRandomReward();
                rewards.Add(reward.GenerateItem());
            }

            return rewards;
        }

        private Reward CreateRandomReward()
        {
            // 获取所有 Reward 的非抽象子类
            var rewardTypes = Assembly.GetAssembly(typeof(Reward))
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Reward)) && !type.IsAbstract)
                .ToList();

            if (rewardTypes.Count == 0)
            {
                throw new InvalidOperationException("No subclasses of Reward found.");
            }

            // 随机选择一个子类并创建实例
            int index = random.Next(rewardTypes.Count);
            return (Reward)Activator.CreateInstance(rewardTypes[index]);
        }
    }
}
