using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Reward;
using UI.Reward;

namespace Controller
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] private Panel rewardPanel;
        [SerializeField] private Controller.TetriResource tetriResource;
        private RewardFactory rewardFactory;
        public event System.Action OnRewardSelected;

        private void Start()
        {
            rewardFactory = new RewardFactory();
        } 

        public void EnterRewardSelectionPhase()
        {
            if (rewardPanel != null)
            {
                List<Item> rewards = rewardFactory.GenerateRewards();
                rewardPanel.SetRewards(rewards); // todo 这里UI知道数据的具体信息,合理吗
                rewardPanel.gameObject.SetActive(true);
                rewardPanel.OnItemSelected += HandleItemSelectd;
            }
        }

        private void HandleItemSelectd(Item reward)
        {
            rewardPanel.OnItemSelected -= HandleItemSelectd;
            ApplyReward(reward);
            rewardPanel.gameObject.SetActive(false);
        }

        private void ApplyReward(Item reward)
        {
            if (reward.GeneratedTetri != null)
            {
                tetriResource.AddUsableTetri(reward.GeneratedTetri);
            }

            Debug.Log("Reward applied: " + reward.Name);
            OnRewardSelected?.Invoke();
        }
    }
}