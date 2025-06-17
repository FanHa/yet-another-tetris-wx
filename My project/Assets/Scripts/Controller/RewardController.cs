using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Rewards;
using UI.Reward;

namespace Controller
{
    public class RewardController : MonoBehaviour
    {
        [SerializeField] private Panel rewardPanel;
        [SerializeField] private Model.TetriInventoryModel tetriInventoryData;

        private RewardFactory rewardFactory;
        public event System.Action OnRewardSelected;

        private void Start()
        {
            rewardFactory = new RewardFactory(tetriInventoryData);
        } 

        public void EnterRewardSelectionPhase()
        {
            if (rewardPanel != null)
            {
                List<Model.Rewards.Reward> rewards = rewardFactory.GenerateRewards();
                rewardPanel.SetRewards(rewards);
                rewardPanel.gameObject.SetActive(true);
                rewardPanel.OnItemSelected += HandleItemSelectd;
            }
        }

        private void HandleItemSelectd(Model.Rewards.Reward reward)
        {
            rewardPanel.OnItemSelected -= HandleItemSelectd;
            ApplyReward(reward);
            rewardPanel.gameObject.SetActive(false);
        }

        private void ApplyReward(Model.Rewards.Reward reward)
        {
            reward.Apply(tetriInventoryData);
            // if (reward is Model.Rewards.AddTetri tetriReward)
            // {
            //     tetriResourceData.AddUsableTetri(tetriReward.GetTetri());
            // }

            // if (reward is Model.Rewards.NewCharacter characterReward)
            // {   
            //     tetriResourceData.AddUsableTetri(characterReward.GetTetri());

            // }
            // if (reward is Model.Rewards.UpgradeTetri upgradeTetriReward)
            // {
            //     upgradeTetriReward.Apply();
            //     // 将 UpgradeTetri 中的 Tetri 移动到 UsableTetri 列表中
            //     var upgradedTetri = upgradeTetriReward.GetTargetTetri();
            //     // 从原来的列表中移除该 Tetri
            //     tetriResourceData.RemoveTetri(upgradedTetri);
            //     tetriResourceData.AddUsableTetri(upgradedTetri);
            // }
            // // todo 其他类型的reward和错误处理
            OnRewardSelected?.Invoke();
        }
    }
}