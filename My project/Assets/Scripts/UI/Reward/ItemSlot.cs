using System;
using Model.Tetri;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Reward
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI detailText;
        [SerializeField] private Transform previewParent; // Parent for the Tetri preview grid

        private Model.Rewards.Reward item;        
        public event Action<ItemSlot> OnItemClicked;

        public void SetReward(Model.Rewards.Reward reward)
        {
            item = reward;
            nameText.text = reward.Name();
            detailText.text = reward.Description();
        }

        public void SetPreview(GameObject preview)
        {
            preview.transform.SetParent(previewParent, false);
        }

        public Model.Rewards.Reward GetReward()
        {
            return item;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerEnter == gameObject)
            {
                Debug.Log("Item clicked: " + item.Name());
                OnItemClicked?.Invoke(this);
            }
        }


    }
}
