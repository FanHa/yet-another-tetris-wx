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
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private Transform gridParent; // Parent for the Tetri preview grid

        private Model.Reward.Item item;
        private Model.Tetri.TetriCellTypeResourceMapping spriteMapping; // Mapping for TetriCellType to Sprite
        
        public event Action<ItemSlot> OnItemClicked;

        public void SetReward(Model.Reward.Item reward, TetriCellTypeResourceMapping mapping)
        {
            item = reward;
            rewardText.text = reward.Name;
            spriteMapping = mapping;

            if (reward.GeneratedTetri != null)
            {
                // CreateGridImages(reward.GeneratedTetri);
            }
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

        // private void CreateGridImages(Model.Tetri.Tetri tetri)
        // {
        //     // Clear existing grid
        //     foreach (Transform child in gridParent)
        //     {
        //         Destroy(child.gameObject);
        //     }

        //     TetrisResourceItemFactory tetrisResourceItemFactory = new TetrisResourceItemFactory();
        //     tetrisResourceItemFactory.CreateInstance(tetri, gridParent, spriteMapping);
        //     // Use TetrisResourceItem's CreateGridImages method
        //     var tempResourceItem = Instantiate(tetrisResourceItemPrefab, gridParent, false);
        //     tempResourceItem.Initialize(tetri, spriteMapping);
        //     Destroy(tempResourceItem.gameObject); // Only use it to generate the grid, then destroy
        // }
    }
}
