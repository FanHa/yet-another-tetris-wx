using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Model.Tetri;
using WeChatWASM;

namespace UI.Reward
{
    public class Panel : MonoBehaviour
    {
        
        public ItemSlot itemPrefab;
        public Transform itemParent;
        public event Action<Model.Rewards.Reward> OnItemSelected;
        [SerializeField] private Controller.Tetris tetris;
        public void SetRewards(List<Model.Rewards.Reward> rewards)
        {
            foreach (Transform child in itemParent)
            {
                Destroy(child.gameObject);
            }
            foreach (Model.Rewards.Reward reward in rewards)
            {
                ItemSlot item = Instantiate(itemPrefab, itemParent);
                
                item.SetReward(reward);
                if (reward is Model.Rewards.AddTetri tetriReward)
                {
                    item.SetPreview(tetris.GenerateTetriPreview(tetriReward.GetTetri()));
                }
                if (reward is Model.Rewards.NewCharacter characterReward)
                {
                    item.SetPreview(tetris.GenerateCharacterPreview(characterReward.GetCharacter()));
                }
                if (reward is Model.Rewards.UpgradeTetri upgradeTetriReward)
                {
                    GameObject upgradePreview = tetris.GenerateUpgradeTetriPreview(
                        upgradeTetriReward.GetTargetTetri(),
                        upgradeTetriReward.GetTargetPosition(),
                        upgradeTetriReward.GetNewCell());
                    item.SetPreview(upgradePreview);
                }
                item.OnItemClicked += HandleItemClicked;
            }
        }
        public void HandleItemClicked(ItemSlot ItemSlot)
        {
            Debug.Log("Item clicked: " + ItemSlot.GetReward());
            OnItemSelected?.Invoke(ItemSlot.GetReward());
        }
    }
}

