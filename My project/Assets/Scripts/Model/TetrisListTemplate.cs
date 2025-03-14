using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model{
    [CreateAssetMenu(fileName = "TetrisListTemplate", menuName = "ScriptableObjects/TetrisListTemplate", order = 1)]
    public class TetrisListTemplate : ScriptableObject
    {
        public List<Tetri.Tetri> template = new(); // 未使用的Tetri列表模板
        private TetriCellFactory _tetriCellFactory = new TetriCellFactory();

        private void OnEnable()
        {   
            template = new();
            GenerateAllShapes();
        }
        private void GenerateAllShapes()
        {
            var shapes = new List<System.Action<Tetri.Tetri>>
            {
                _tetriCellFactory.CreateTShape,
                _tetriCellFactory.CreateIShape,
                _tetriCellFactory.CreateOShape,
                _tetriCellFactory.CreateLShape,
                _tetriCellFactory.CreateJShape,
                _tetriCellFactory.CreateSShape,
                _tetriCellFactory.CreateZShape,
                _tetriCellFactory.CreateUShape, // 新增的U形
                _tetriCellFactory.CreateVShape, // 新增的V形
                _tetriCellFactory.CreateWShape, // 新增的W形
                _tetriCellFactory.CreateXShape  // 新增的X形
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

        private void ReplaceRandomCell(Tetri.Tetri tetri)
        {
            var random = new System.Random();
            var cells = new List<(int, int)>();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (tetri.Shape[i, j] is not TetriCellEmpty)
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