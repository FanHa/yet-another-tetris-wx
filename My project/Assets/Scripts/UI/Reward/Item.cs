using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Reward
{
    public class Item : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text rewardText;
        private string reward;
        // private Panel panel;

        public event Action<Item> OnItemClicked;

        public void SetReward(string reward)
        {
            this.reward = reward;
            rewardText.text = reward;
        }

        public string GetReward()
        {
            return reward;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerEnter == gameObject)
            {
                Debug.Log("Item clicked: " + reward);
                OnItemClicked?.Invoke(this);
            }
        }
    }
}
