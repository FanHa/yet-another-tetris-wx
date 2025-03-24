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
            var rewardTypes = new List<Func<BaseReward>>
            {
                () => new AttackReward(),
                // () => new HealthReward(),
                // () => new HeavyReward(),
                // () => new SpeedReward(),
                () => new RangeAttack()
            };

            int index = random.Next(rewardTypes.Count);
            return rewardTypes[index]();
        }
    }
}
