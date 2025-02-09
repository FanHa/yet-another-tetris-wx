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
            tetri.SetCell(1, 0, new TetriCellBasic());
            tetri.SetCell(1, 1, new TetriCellBasic());
            tetri.SetCell(1, 2, new TetriCellBasic());
            tetri.SetCell(0, 1, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
        }

        private void CreateIShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, new TetriCellBasic());
            tetri.SetCell(1, 1, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
            tetri.SetCell(3, 1, new TetriCellBasic());
        }

        private void CreateOShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 0, new TetriCellBasic());
            tetri.SetCell(0, 1, new TetriCellBasic());
            tetri.SetCell(1, 0, new TetriCellBasic());
            tetri.SetCell(1, 1, new TetriCellBasic());
        }

        private void CreateLShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, new TetriCellBasic());
            tetri.SetCell(1, 1, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
            tetri.SetCell(2, 2, new TetriCellBasic());
        }

        private void CreateJShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 1, new TetriCellBasic());
            tetri.SetCell(1, 1, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
            tetri.SetCell(2, 0, new TetriCellBasic());
        }

        private void CreateSShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 0, new TetriCellBasic());
            tetri.SetCell(1, 1, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
            tetri.SetCell(2, 2, new TetriCellBasic());
        }

        private void CreateZShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 1, new TetriCellBasic());
            tetri.SetCell(1, 2, new TetriCellBasic());
            tetri.SetCell(2, 0, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
        }

        // 新增的U形
        private void CreateUShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 0, new TetriCellBasic());
            tetri.SetCell(0, 2, new TetriCellBasic());
            tetri.SetCell(1, 0, new TetriCellBasic());
            tetri.SetCell(1, 2, new TetriCellBasic());
            tetri.SetCell(2, 0, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
            tetri.SetCell(2, 2, new TetriCellBasic());
        }

        // 新增的V形
        private void CreateVShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(0, 0, new TetriCellBasic());
            tetri.SetCell(1, 0, new TetriCellBasic());
            tetri.SetCell(2, 0, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
            tetri.SetCell(2, 2, new TetriCellBasic());
            tetri.SetCell(1, 2, new TetriCellBasic());
            tetri.SetCell(0, 2, new TetriCellBasic());
        }

        // 新增的W形
        private void CreateWShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 0, new TetriCellBasic());
            tetri.SetCell(1, 1, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
            tetri.SetCell(2, 2, new TetriCellBasic());
            tetri.SetCell(3, 2, new TetriCellBasic());
        }

        // 新增的X形
        private void CreateXShape(Tetri.Tetri tetri)
        {
            tetri.SetCell(1, 1, new TetriCellBasic());
            tetri.SetCell(0, 1, new TetriCellBasic());
            tetri.SetCell(1, 0, new TetriCellBasic());
            tetri.SetCell(1, 2, new TetriCellBasic());
            tetri.SetCell(2, 1, new TetriCellBasic());
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
                tetri.SetCell(row, col, random.Next(2) == 0 ? new TetriCellCharacterCircle() : new TetriCellCharacterTriangle());
            }
        }
    }
}