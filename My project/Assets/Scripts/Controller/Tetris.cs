using UnityEngine;
using UnityEngine.UI;
using Model.Tetri;
using System;

namespace Controller {
    public class Tetris : MonoBehaviour {
        [SerializeField] private GameObject previewPrefab;
        [SerializeField] private GameObject tetriCellPrefab;
        [SerializeField] private TetriCellTypeResourceMapping spriteMapping;
        [SerializeField] private Color colorNew;
        [SerializeField] private Color colorUpgrade;

        public GameObject GenerateTetriPreview(Tetri tetri, Vector2? gridSize = null)
        {
            GameObject preview = Instantiate(previewPrefab);

            // Get GridLayoutGroup component
            GridLayoutGroup gridLayoutGroup = preview.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup != null && gridSize != null)
            {
                gridLayoutGroup.cellSize = gridSize.Value;
            }

            // Create 4x4 grid of images
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject brick = Instantiate(tetriCellPrefab, preview.transform);
                    UI.Cell cellComponent = brick.GetComponent<UI.Cell>();

                    // Set image display based on Tetri shape
                    Cell cell = tetri.Shape[i, j];
                    if (cell is not Empty)
                    {
                        Sprite sprite = spriteMapping.GetSprite(cell.GetType());
                        if (sprite != null)
                        {
                            cellComponent.SetImage(sprite); // Set the image for the cell
                            cellComponent.SetCellOutLineColor(colorNew); // Show outline
                        }
                    }
                    else 
                    {
                        cellComponent.SetTransparent(); // Set the cell to be transparent
                    }
                }
            }

            return preview;
        }

        public GameObject GenerateCharacterPreview(Cell character, Vector2? gridSize = null)
        {
            GameObject preview = Instantiate(tetriCellPrefab);
            UI.Cell cellComponent = preview.GetComponent<UI.Cell>();

            Sprite sprite = spriteMapping.GetSprite(character);
            cellComponent.SetImage(sprite); // Set the image for the character cell
            cellComponent.SetCellOutLineColor(colorNew); // Show outline
            return preview;
        }

        public GameObject GenerateUpgradeTetriPreview(Tetri tetri, Vector2Int targetPosition, Cell newCell, Vector2? gridSize = null)
        {
            GameObject preview = Instantiate(previewPrefab);

            // 获取 GridLayoutGroup 组件
            GridLayoutGroup gridLayoutGroup = preview.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup != null && gridSize != null)
            {
                gridLayoutGroup.cellSize = gridSize.Value;
            }

            // 创建 4x4 网格的图像
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject brick = Instantiate(tetriCellPrefab, preview.transform);
                    UI.Cell cellComponent = brick.GetComponent<UI.Cell>();

                    // 获取当前单元格
                    Cell cell = tetri.Shape[i, j];

                    // 如果是目标位置，显示新生成的 Cell
                    if (i == targetPosition.x && j == targetPosition.y)
                    {
                        Sprite sprite = spriteMapping.GetSprite(newCell.GetType());
                        if (sprite != null)
                        {
                            cellComponent.SetImage(sprite); // 设置新单元格的图像
                            cellComponent.SetCellOutLineColor(colorUpgrade); // 显示轮廓
                        }
                    }
                    else if (cell is not Empty) // 如果是普通的非空单元格
                    {
                        Sprite sprite = spriteMapping.GetSprite(cell.GetType());
                        if (sprite != null)
                        {
                            cellComponent.SetImage(sprite);
                        }
                    }
                    else 
                    {
                        cellComponent.SetTransparent(); // Set the cell to be transparent
                    }
                }
            }

            return preview;
        }

    }
}