using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Reward
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text rewardText;

        private Model.Reward.Item item;
        // private Panel panel;

        public event Action<ItemSlot> OnItemClicked;

        public void SetReward(Model.Reward.Item reward)
        {
            item = reward;
            // rewardText.text = reward;
        }

        public Model.Reward.Item GetReward()
        {
            return item;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerEnter == gameObject)
            {
                Debug.Log("Item clicked: " + item);
                OnItemClicked?.Invoke(this);
            }
        }
    }
}
