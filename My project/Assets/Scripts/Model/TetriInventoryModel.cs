using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;
using WeChatWASM;

namespace Model
{
    [CreateAssetMenu(fileName = "TetriInventoryModel", menuName = "SO/TetriInventoryModel", order = 1)]

    public class TetriInventoryModel : ScriptableObject
    {
        public event Action OnDataChanged;

        [SerializeField] private List<Model.Tetri.Tetri> usableTetriList = new List<Model.Tetri.Tetri>();
        [SerializeField] private List<Model.Tetri.Tetri> usedTetriList = new List<Model.Tetri.Tetri>();

        public IReadOnlyList<Model.Tetri.Tetri> UsableTetriList => usableTetriList;
        public IReadOnlyList<Model.Tetri.Tetri> UsedTetriList => usedTetriList;

        private List<Type> attributeTypes; // 用于随机替换单元格的类型
        private Model.Tetri.TetrisFactory tetrisFactory = new Model.Tetri.TetrisFactory();
        private HashSet<CellGroupConfig.Group> tetriGroups = new HashSet<CellGroupConfig.Group>();
        public IReadOnlyCollection<CellGroupConfig.Group> TetriGroups => tetriGroups; // 只读访问
        private HashSet<Type> cellTypes = new HashSet<Type>();
        public IReadOnlyCollection<Type> CellTypes => cellTypes; // 只读访问
        public void Init()
        {
            InitializeAttributeTypes();
            // 先清空列表，避免重复
            usableTetriList.Clear();
            usedTetriList.Clear();
            GenerateInitialTetris();
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

        private void GenerateInitialTetris()
        {
            // 假设需要生成固定数量的随机 Tetri
            int initialTetriCount = 7; // 或者根据需求调整数量

            // 确保每种 attributeType 至少在一个 Tetri 中出现
            foreach (var attributeType in attributeTypes)
            {
                var tetri = tetrisFactory.CreateRandomBaseShape(); // 使用工厂的随机生成方法
                ReplaceSpecificCell(tetri, attributeType); // 确保包含该 attributeType
                usableTetriList.Add(tetri);
            }

            // 生成剩余的随机 Tetri
            for (int i = attributeTypes.Count; i < initialTetriCount; i++)
            {
                var tetri = tetrisFactory.CreateRandomBaseShape(); // 使用工厂的随机生成方法
                ReplaceRandomCell(tetri); // 随机替换单元格
                usableTetriList.Add(tetri);
            }

            RecalculateCellTypes();
        }

        private void ReplaceSpecificCell(Tetri.Tetri tetri, Type attributeType)
        {
            ReplaceCell(tetri, attributeType);
        }

        private void ReplaceCell(Tetri.Tetri tetri, Type specificType = null)
        {
            var random = new System.Random();
            var cells = tetri.GetOccupiedPositions(); // 获取非空单元格的位置

            if (cells.Count > 0)
            {
                // 随机选择一个非空单元格的位置
                var randomCellPosition = cells[random.Next(cells.Count)];

                // 如果指定了特定类型，则使用该类型；否则随机选择一个类型
                var cellType = specificType ?? attributeTypes[random.Next(attributeTypes.Count)];

                // 创建指定类型的实例
                var attributeInstance = (Cell)Activator.CreateInstance(cellType);

                // 将实例设置到随机单元格中
                tetri.SetCell(randomCellPosition.x, randomCellPosition.y, attributeInstance);
            }
        }

        private void ReplaceRandomCell(Tetri.Tetri tetri)
        {
            ReplaceCell(tetri);
        }

        private void RecalculateCellTypes()
        {
            cellTypes.Clear(); // 清空当前的 cellTypes
            tetriGroups.Clear();

            // 遍历所有 Tetri 列表，重新计算包含的 Cell 类型
            foreach (var tetri in usableTetriList.Concat(usedTetriList))
            {
                foreach (var position in tetri.GetOccupiedPositions())
                {
                    Cell cell = tetri.Shape[position.x, position.y];
                    cellTypes.Add(cell.GetType());
                }
                tetriGroups.Add(tetri.Group);
            }

            // 触发数据变化事件
            OnDataChanged?.Invoke();
        }
        
        public void MarkTetriAsUsed(Model.Tetri.Tetri tetri)
        {
            if (usableTetriList.Contains(tetri))
            {
                usableTetriList.Remove(tetri);
                usedTetriList.Add(tetri);

                // 触发数据变化事件
                OnDataChanged?.Invoke();
            }
            else
            {
                Debug.LogWarning("Tetri is not in the usable list and cannot be marked as used.");
            }
        }


    }
}