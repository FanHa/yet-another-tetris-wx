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
        private TetrisFactory tetrisFactory = new TetrisFactory();
        private List<Type> attributeTypes = new();
        private void OnEnable()
        {   
            template = new();
            GenerateAllShapes();
            // 初始化缓存的 attributeTypes
            attributeTypes = typeof(Model.Tetri.Attribute).Assembly
                .GetTypes()
                .Where(type => typeof(Model.Tetri.Attribute).IsAssignableFrom(type) // 检查是否实现了接口
                                && !type.IsAbstract // 排除抽象类
                                && !type.IsInterface) // 排除接口本身
                .ToList();
            
        }
        private void GenerateAllShapes()
        {
            var shapes = new List<System.Func<Model.Tetri.Tetri>>
            {
                tetrisFactory.CreateTShape,
                tetrisFactory.CreateIShape,
                tetrisFactory.CreateOShape,
                tetrisFactory.CreateLShape,
                tetrisFactory.CreateJShape,
                tetrisFactory.CreateSShape,
                tetrisFactory.CreateZShape,
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

        private void EnsureAttributeTypesInitialized()
        {
            if (attributeTypes == null || attributeTypes.Count == 0)
            {
                attributeTypes = typeof(Model.Tetri.Attribute).Assembly
                    .GetTypes()
                    .Where(type => typeof(Model.Tetri.Attribute).IsAssignableFrom(type) // 检查是否实现了接口
                                    && !type.IsAbstract // 排除抽象类
                                    && !type.IsInterface) // 排除接口本身
                    .ToList();
            }
        }

        private void ReplaceRandomCell(Tetri.Tetri tetri)
        {
            EnsureAttributeTypesInitialized();
            var random = new System.Random();
            var cells = new List<(int, int)>();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (tetri.Shape[i, j] is not Empty)
                    {
                        cells.Add((i, j));
                    }
                }
            }

            if (cells.Count > 0)
            {
                var (row, col) = cells[random.Next(cells.Count)];


                if (attributeTypes.Count > 0)
                {
                    // 随机选择一个子类并创建实例
                    Type selectedType = attributeTypes[random.Next(attributeTypes.Count)];
                    Model.Tetri.Cell attributeInstance = (Cell)Activator.CreateInstance(selectedType);

                    // 替换单元格
                    tetri.SetCell(row, col, attributeInstance);
                }
            }
        }
    }
}