using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace UI.Reward
{
    public class Panel : MonoBehaviour
    {
        public GameObject itemPrefab;
        public Transform itemParent;
        public event Action<string> OnRewardSelected;
        public void SetRewards(List<string> rewards)
        {
            foreach (Transform child in itemParent)
            {
                Destroy(child.gameObject);
            }
            foreach (string reward in rewards)
            {
                GameObject itemObject = Instantiate(itemPrefab, itemParent);
                Item item = itemObject.GetComponent<Item>();
                item.SetReward(reward);
                item.OnItemClicked += HandleItemClicked;
            }
        }
        public void HandleItemClicked(Item item)
        {
            Debug.Log("Item clicked: " + item.GetReward());
            OnRewardSelected?.Invoke(item.GetReward());
        }
    }
}

