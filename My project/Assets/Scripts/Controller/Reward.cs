using System.Collections.Generic;
using UnityEngine;
using Model;
using UI.Reward;

namespace Controller
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] private Panel rewardPanel;
        [SerializeField] private Controller.TetriResource tetriResource;
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
            List<Model.Reward.Item> rewards = new List<Model.Reward.Item>();
            var tetriFactory = new Model.Tetri.TetriCellFactory();

            for (int i = 1; i <= 3; i++)
            {
                var tetri = new Model.Tetri.Tetri();
                tetriFactory.CreateTShape(tetri); // Example: Generate a T-shaped Tetri
                rewards.Add(new Model.Reward.Item($"Reward{i}", $"Description{i}", tetri));
            }

            return rewards;
        }

        private void ApplyReward(Model.Reward.Item reward)
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