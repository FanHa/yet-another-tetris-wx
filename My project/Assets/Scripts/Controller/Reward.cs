using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Rewards;
using UI.Reward;

namespace Controller
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] private Panel rewardPanel;
        [SerializeField] private Controller.Resource tetriResource;
        [SerializeField] private Model.OperationTable operationTableData;
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
                List<Model.Rewards.Reward> rewards = rewardFactory.GenerateRewards();
                rewardPanel.SetRewards(rewards); // todo 这里UI知道数据的具体信息,合理吗
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
            if (reward is Model.Rewards.Tetri tetriReward)
            {
                tetriResource.AddUsableTetri(tetriReward.GetTetri());
            }

            if (reward is Model.Rewards.Character characterReward)
            {
                operationTableData.PlaceCharacterInRandomRow(characterReward.GetCharacter());
            }
            // todo 其他类型的reward和错误处理
            Debug.Log("Reward applied: " + reward.GetName());
            OnRewardSelected?.Invoke();
        }
    }
}