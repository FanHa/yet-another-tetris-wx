using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model{
    [CreateAssetMenu(fileName = "TetrisListTemplate", menuName = "ScriptableObjects/TetrisListTemplate", order = 1)]
    public class TetrisListTemplate : ScriptableObject
    {
        public List<Tetri.Tetri> template = new(); // 未使用的Tetri列表模板

        private void OnEnable()
        {   
            template = new();
            GenerateAllShapes();
        }
        private void GenerateAllShapes()
        {
            var shapes = new List<System.Action<Tetri.Tetri>>
            {
                CreateTShape,
                CreateIShape,
                CreateOShape,
                CreateLShape,
                CreateJShape,
                CreateSShape,
                CreateZShape,
                CreateUShape, // 新增的U形
                CreateVShape, // 新增的V形
                CreateWShape, // 新增的W形
                CreateXShape  // 新增的X形
            };

            foreach (var createShape in shapes)
            {
                var tetri = new Tetri.Tetri();
                tetri.InitializeShape();
                createShape(tetri);
                ReplaceRandomCell(tetri);
                template.Add(tetri);
            }
        }

        private void CreateTShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(1, 2, CreateBasicCell());
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
        }

        private void CreateIShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(3, 1, CreateBasicCell());
        }

        private void CreateOShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 0, CreateBasicCell());
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
        }

        private void CreateLShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 2, CreateBasicCell());
        }

        private void CreateJShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 0, CreateBasicCell());
        }

        private void CreateSShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 2, CreateBasicCell());
        }

        private void CreateZShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(1, 2, CreateBasicCell());
            tetri.SetCell(2, 0, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
        }

        // 新增的U形
        private void CreateUShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 0, CreateBasicCell());
            tetri.SetCell(0, 2, CreateBasicCell());
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 2, CreateBasicCell());
            tetri.SetCell(2, 0, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 2, CreateBasicCell());
        }

        // 新增的V形
        private void CreateVShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 0, CreateBasicCell());
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(2, 0, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 2, CreateBasicCell());
            tetri.SetCell(1, 2, CreateBasicCell());
            tetri.SetCell(0, 2, CreateBasicCell());
        }

        // 新增的W形
        private void CreateWShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
            tetri.SetCell(2, 2, CreateBasicCell());
            tetri.SetCell(3, 2, CreateBasicCell());
        }

        // 新增的X形
        private void CreateXShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 1, CreateBasicCell());
            tetri.SetCell(0, 1, CreateBasicCell());
            tetri.SetCell(1, 0, CreateBasicCell());
            tetri.SetCell(1, 2, CreateBasicCell());
            tetri.SetCell(2, 1, CreateBasicCell());
        }

        private void ReplaceRandomCell(Tetri.Tetri tetri)
        {
            var random = new System.Random();
            var cells = new List<(int, int)>();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (tetri.Shape[i, j] is TetriCellBasic)
                    {
                        cells.Add((i, j));
                    }
                }
            }

            if (cells.Count > 0)
            {
                var (row, col) = cells[random.Next(cells.Count)];
                var possibleCells = new List<TetriCell>
                {
                    // new TetriCellCharacterCircle(),
                    // new TetriCellCharacterTriangle(),
                    new TetriCellAttributeHealth(),
                    new TetriCellAttributeAttack(),
                    new TetriCellAttributeHeavy(),
                    new TetriCellAttributeSpeed()
                };
                tetri.SetCell(row, col, possibleCells[random.Next(possibleCells.Count)]);
            }
        }

        private TetriCell CreateBasicCell()
        {
            var possibleCells = new List<TetriCell>
            {
                new TetriCellCharacterCircle(),
                new TetriCellCharacterTriangle(),
                new TetriCellCharacterSquare()
            };
            var random = new System.Random();
            return possibleCells[random.Next(possibleCells.Count)];
        }
    }
}