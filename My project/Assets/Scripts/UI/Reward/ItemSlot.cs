using System;
using Model.Tetri;
using TMPro;
using UI.TetrisResource;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Reward
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private Transform previewParent; // Parent for the Tetri preview grid

        private Model.Reward.Item item;        
        public event Action<ItemSlot> OnItemClicked;

        public void SetReward(Model.Reward.Item reward, GameObject preview)
        {
            item = reward;
            rewardText.text = reward.Name;
            preview.transform.SetParent(previewParent, false);
        }

        public Model.Reward.Item GetReward()
        {
            return item;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerEnter == gameObject)
            {
                Debug.Log("Item clicked: " + item.Name);
                OnItemClicked?.Invoke(this);
            }
        }


    }
}
