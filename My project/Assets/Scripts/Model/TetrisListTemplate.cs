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
        private List<Type> attributeTypes;
        private void OnEnable()
        {   
            template.Clear();
            InitializeAttributeTypes();
            GenerateAllShapes();
            
        }

        private void GenerateAllShapes()
        {
            // 假设需要生成固定数量的随机形状
            int shapeCount = 7; // 或者根据需求调整数量
            for (int i = 0; i < shapeCount; i++)
            {
                var tetri = tetrisFactory.CreateRandomShape(); // 使用工厂的随机生成方法
                ReplaceRandomCell(tetri);
                template.Add(tetri);
            }
        }

        private void InitializeAttributeTypes()
        {
            // 初始化 attributeTypes，仅在为空时加载一次
            attributeTypes ??= typeof(Model.Tetri.Attribute).Assembly
                .GetTypes()
                .Where(type => typeof(Model.Tetri.Attribute).IsAssignableFrom(type) // 检查是否实现了接口
                                && !type.IsAbstract // 排除抽象类
                                && !type.IsInterface) // 排除接口本身
                .ToList();
        }
        private void ReplaceRandomCell(Tetri.Tetri tetri)
        {
            var random = new System.Random();
            var cells = tetri.GetOccupiedPositions(); // 获取非空单元格的位置

            if (cells.Count > 0)
            {
                // 随机选择一个非空单元格的位置
                var randomCellPosition = cells[random.Next(cells.Count)];

                // 随机从 attributeTypes 中选择一个类型
                var randomAttributeType = attributeTypes[random.Next(attributeTypes.Count)];

                // 创建该类型的实例
                var attributeInstance = (Cell)Activator.CreateInstance(randomAttributeType);

                // 将实例设置到随机单元格中
                tetri.SetCell(randomCellPosition.x, randomCellPosition.y, attributeInstance);
            }
        }
    }
}