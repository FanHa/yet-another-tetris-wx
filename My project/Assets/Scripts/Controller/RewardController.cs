using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Rewards;
using UI.Reward;

namespace Controller
{
    [RequireComponent(typeof(RewardPanelView))]
    public class RewardController : MonoBehaviour
    {
        private RewardPanelView rewardPanelView;
        [SerializeField] private Model.TetriInventoryModel tetriInventoryData;

        [SerializeField] private RewardFactory rewardFactory;
        public event System.Action OnRewardSelected;
        private void Awake()
        {
            rewardPanelView = GetComponent<RewardPanelView>();
            if (rewardPanelView == null)
            {
                Debug.LogError("RewardPanelView component is missing on RewardController.");
            }
        }

        public void EnterRewardSelectionPhase()
        {

            List<Model.Rewards.Reward> rewards = rewardFactory.GenerateRewards();
            rewardPanelView.SetRewards(rewards);
            rewardPanelView.ShowItems();
            rewardPanelView.OnItemSelected += HandleItemSelected;
            
        }

        private void HandleItemSelected(Model.Rewards.Reward reward)
        {
            rewardPanelView.OnItemSelected -= HandleItemSelected;
            ApplyReward(reward);
            rewardPanelView.gameObject.SetActive(false);
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