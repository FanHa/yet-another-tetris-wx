using System;
using System.Collections;
using System.Collections.Generic;
using Model.Tetri;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventories.Description
{
    public class Description : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Item descriptionItemPrefab;
        [SerializeField] private Transform descriptionItemParent; // 父对象，用于存放DescriptionItem
        [SerializeField] private TetriCellTypeResourceMapping cellTypeResourceMapping;

        public void ResetDescription()
        {
            itemImage.gameObject.SetActive(false);
            // 清空现有的DescriptionItem
            foreach (Transform child in descriptionItemParent)
            {
                Destroy(child.gameObject);
            }
        }

        public void SetDescription(Model.UnitInventoryItem item)
        {
            // 清空现有的DescriptionItem
            foreach (Transform child in descriptionItemParent)
            {
                Destroy(child.gameObject);
            }
            itemImage.sprite = cellTypeResourceMapping.GetSprite(item.CharacterCell);
            itemImage.gameObject.SetActive(true);
            descriptionText.text = item.CharacterCell.Description();

             // 对 TetriCells 进行分组，按类型合并并记录数量
            Dictionary<Type, int> groupedCells = new Dictionary<System.Type, int>();
            foreach (Model.Tetri.Cell cell in item.TetriCells)
            {
                var cellType = cell.GetType(); // 获取 Cell 的类型
                if (groupedCells.ContainsKey(cellType))
                {
                    groupedCells[cellType]++;
                }
                else
                {
                    groupedCells[cellType] = 1;
                }
            }

            // 遍历分组后的结果，为每种类型的 Cell 创建一个 DescriptionItem
            foreach (var kvp in groupedCells)
            {
                System.Type cellType = kvp.Key;
                int count = kvp.Value;

                // 创建一个新的 DescriptionItem 实例
                Item newItem = Instantiate(descriptionItemPrefab, descriptionItemParent);

                // 获取类型对应的一个实例（可以从 TetriCells 中找到第一个匹配的实例）
                Model.Tetri.Cell exampleCell = item.TetriCells.Find(c => c.GetType() == cellType);

                // 设置 DescriptionItem 的属性，包括图片、描述和数量
                newItem.SetDescription(cellTypeResourceMapping.GetSprite(exampleCell), count, exampleCell.Description());
            }

        }
    }
}
