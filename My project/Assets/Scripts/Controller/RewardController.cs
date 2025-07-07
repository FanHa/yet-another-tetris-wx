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
            rewardPanelView.gameObject.SetActive(true);
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
            OnRewardSelected?.Invoke();
        }

#if UNITY_EDITOR
        [ContextMenu("Test Enter Reward Selection Phase")]
        private void TestEnterRewardSelectionPhase()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("This method can only be called in Play mode.");
                return;
            }
            EnterRewardSelectionPhase();
        }
#endif
    }
}