using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Model{
    [CreateAssetMenu(fileName = "TetrisListTemplate", menuName = "ScriptableObjects/TetrisListTemplate", order = 1)]
    public class TetrisListTemplate : ScriptableObject
    {
        public List<Tetri.Tetri> template = new(); // 未使用的Tetri列表模板
        private Model.Tetri.TetrisFactory _tetrisFactory = new Model.Tetri.TetrisFactory();

        private void OnEnable()
        {   
            template = new();
            GenerateAllShapes();
        }
        private void GenerateAllShapes()
        {
            var shapes = new List<System.Func<Model.Tetri.Tetri>>
            {
                _tetrisFactory.CreateTShape,
                _tetrisFactory.CreateIShape,
                _tetrisFactory.CreateOShape,
                _tetrisFactory.CreateLShape,
                _tetrisFactory.CreateJShape,
                _tetrisFactory.CreateSShape,
                _tetrisFactory.CreateZShape,
            };

            var random = new System.Random();
            for (int i = 0; i < shapes.Count; i++)
            {
                var createShape = shapes[random.Next(shapes.Count)];
                Model.Tetri.Tetri tetri = createShape();
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

                // 动态获取所有 TetriCellAttribute 的子类
                var attributeTypes = typeof(TetriCellAttribute).Assembly
                    .GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(TetriCellAttribute)) && !type.IsAbstract)
                    .ToList();

                if (attributeTypes.Count > 0)
                {
                    // 随机选择一个子类并创建实例
                    var selectedType = attributeTypes[random.Next(attributeTypes.Count)];
                    var attributeInstance = (TetriCell)Activator.CreateInstance(selectedType);

                    // 替换单元格
                    tetri.SetCell(row, col, attributeInstance);
                }
            }
        }
    }
}