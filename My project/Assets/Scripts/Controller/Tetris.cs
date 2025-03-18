using UnityEngine;
using UnityEngine.UI;
using Model.Tetri;
using System;

namespace Controller {
    public class Tetris : MonoBehaviour {
        [SerializeField] private GameObject previewPrefab;
        [SerializeField] private GameObject tetriBrickPrefab;
        [SerializeField] private TetriCellTypeResourceMapping spriteMapping;
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
                    GameObject brick = Instantiate(tetriBrickPrefab, preview.transform);
                    Image image = brick.GetComponent<Image>();

                    // Set image display based on Tetri shape
                    TetriCell cell = tetri.Shape[i, j];
                    if (cell is not TetriCellEmpty)
                    {
                        Sprite sprite = spriteMapping.GetSprite(cell.GetType());
                        if (sprite != null)
                        {
                            image.sprite = sprite;
                            image.color = Color.green; // Set green color for blocks
                        }
                        else
                        {
                            image.color = Color.black; // Set black color if sprite is missing
                        }
                    }
                    else
                    {
                        image.color = Color.clear; // Set transparent for empty cells
                    }
                }
            }

            return preview;
        }
    }
}