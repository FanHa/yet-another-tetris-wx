using UnityEngine;
using UnityEngine.UI;
using Model.Tetri;
using System;

namespace Controller {
    public class Tetris : MonoBehaviour {
        [SerializeField] private GameObject previewPrefab;
        [SerializeField] private GameObject tetriBrickPrefab;
        [SerializeField] private TetriCellTypeResourceMapping spriteMapping;

        private TetrisFactory tetrisFactory = new TetrisFactory();

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

        public Tetri CreateTShape() => tetrisFactory.CreateTShape();
        public Tetri CreateIShape() => tetrisFactory.CreateIShape();
        public Tetri CreateOShape() => tetrisFactory.CreateOShape();
        public Tetri CreateLShape() => tetrisFactory.CreateLShape();
        public Tetri CreateJShape() => tetrisFactory.CreateJShape();
        public Tetri CreateSShape() => tetrisFactory.CreateSShape();
        public Tetri CreateZShape() => tetrisFactory.CreateZShape();
    }

    public class TetrisFactory {
        private TetriCellFactory cellFactory = new TetriCellFactory();

        public Tetri CreateTShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(1, 0, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 2, cellFactory.CreateBasicCell());
            tetri.SetCell(0, 1, cellFactory.CreateBasicCell());
            return tetri;
        }

        public Tetri CreateIShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(0, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(3, 1, cellFactory.CreateBasicCell());
            return tetri;
        }

        public Tetri CreateOShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(0, 0, cellFactory.CreateBasicCell());
            tetri.SetCell(0, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 0, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 1, cellFactory.CreateBasicCell());
            return tetri;
        }

        public Tetri CreateLShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(0, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 2, cellFactory.CreateBasicCell());
            return tetri;
        }

        public Tetri CreateJShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(0, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 0, cellFactory.CreateBasicCell());
            return tetri;
        }

        public Tetri CreateSShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(1, 0, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 2, cellFactory.CreateBasicCell());
            return tetri;
        }

        public Tetri CreateZShape()
        {
            Tetri tetri = new Tetri();
            tetri.SetCell(1, 1, cellFactory.CreateBasicCell());
            tetri.SetCell(1, 2, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 0, cellFactory.CreateBasicCell());
            tetri.SetCell(2, 1, cellFactory.CreateBasicCell());
            return tetri;
        }
    }
}