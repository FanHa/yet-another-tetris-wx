using System.Collections.Generic;
using UnityEngine;
using Model;
using UI.Reward;

namespace Controller
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] private Panel rewardPanel;
        public event System.Action OnRewardSelected;

        public void EnterRewardSelectionPhase()
        {
            if (rewardPanel != null)
            {
                List<Model.Reward.Item> rewards = GenerateRewards();
                rewardPanel.SetRewards(rewards); // todo 这里UI知道数据的具体信息,合理吗
                rewardPanel.gameObject.SetActive(true);
                rewardPanel.OnItemSelected += HandleItemSelectd;
            }
        }

        private void HandleItemSelectd(Model.Reward.Item reward)
        {
            rewardPanel.OnItemSelected -= HandleItemSelectd;
            ApplyReward(reward);
            rewardPanel.gameObject.SetActive(false);
        }

        public List<Model.Reward.Item> GenerateRewards()
        {
            // 生成奖励列表的逻辑
            List<Model.Reward.Item> rewards = new List<Model.Reward.Item>
            {
                new Model.Reward.SpecificRewardItem("Reward1", "Description1"),
                new Model.Reward.SpecificRewardItem("Reward2", "Description2"),
                new Model.Reward.SpecificRewardItem("Reward3", "Description3")
            };
            return rewards;
        }

        public void ApplyReward(Model.Reward.Item reward)
        {
            reward.ApplyReward();
            Debug.Log("Reward applied: " + reward.Name);
            OnRewardSelected?.Invoke();

        }
    }
}