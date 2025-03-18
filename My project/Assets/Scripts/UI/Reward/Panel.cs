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
        public event Action<Model.Reward.Item> OnItemSelected;
        [SerializeField] private Controller.Tetris tetris;
        public void SetRewards(List<Model.Reward.Item> rewards)
        {
            foreach (Transform child in itemParent)
            {
                Destroy(child.gameObject);
            }
            foreach (Model.Reward.Item reward in rewards)
            {
                ItemSlot item = Instantiate(itemPrefab, itemParent);
                // ItemSlot item = itemObject.GetComponent<ItemSlot>();
                
                item.SetReward(reward, tetris.GenerateTetriPreview(reward.GeneratedTetri));
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

