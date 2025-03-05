using System;
using System.Collections;
using System.Collections.Generic;
using Model.Tetri;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class InventoryDescription : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private DescriptionItem descriptionItemPrefab;
        [SerializeField] private Transform descriptionItemParent; // 父对象，用于存放DescriptionItem
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;

        public void Awake()
        {
            ResetDescription();
        }

        public void ResetDescription()
        {
            itemImage.gameObject.SetActive(false);
        }

        public void SetDescription(Model.InventoryItem item)
        {
            // 遍历item里的TetriCells，为每一个Cell创建一个DescriptionItem
            foreach (TetriCell cell in item.tetriCells)
            {
                // 创建一个新的DescriptionItem实例
                DescriptionItem newItem = Instantiate(descriptionItemPrefab, descriptionItemParent);
                // 设置DescriptionItem的属性
                newItem.SetDescription(cellTypeResourceMapping.GetSprite(cell), cell.Description());
            }
        }
    }
}
