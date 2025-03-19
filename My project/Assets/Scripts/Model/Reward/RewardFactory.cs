using System;
using System.Collections.Generic;
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
                BaseReward reward = CreateRandomReward();
                rewards.Add(reward.GenerateItem());
            }

            return rewards;
        }

        private BaseReward CreateRandomReward()
        {
            return random.Next(4) switch
            {
                0 => new AttackReward(),
                1 => new HealthReward(),
                2 => new HeavyReward(),
                3 => new SpeedReward(),
                _ => new AttackReward() // Default case
            };
        }
    }
}
