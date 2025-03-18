using System.Collections.Generic;
using UnityEngine;
using Model;
using UI.Reward;
using Model.Tetri;

namespace Controller
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] private Panel rewardPanel;
        [SerializeField] private Controller.TetriResource tetriResource;
        private Controller.TetrisFactory tetrisFactory;
        public event System.Action OnRewardSelected;

        private void Start()
        {
            tetrisFactory = new TetrisFactory();
        } 

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

            for (int i = 1; i <= 3; i++)
            {
                Tetri tetri = tetrisFactory.CreateIShape();
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