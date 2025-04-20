using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Model.Tetri;



namespace Model {
    [CreateAssetMenu(fileName = "TetrisResources", menuName = "ScriptableObjects/TetrisResources", order = 1)]
    public class TetrisResources : ScriptableObject
    {
        public event Action OnDataChanged;

        [SerializeField]
        private List<Tetri.Tetri> tetriList = new List<Tetri.Tetri>(); // 管理Tetri列表
        [SerializeField]
        private List<Tetri.Tetri> usedTetriList = new List<Tetri.Tetri>(); // 已使用的Tetri列表
        [SerializeField]
        private List<Tetri.Tetri> unusedTetriList = new List<Tetri.Tetri>(); // 未使用的Tetri列表
        // 用于跟踪当前存在的 Cell 类型
        private HashSet<Type> cellTypes = new HashSet<Type>();

        public IReadOnlyCollection<Type> CellTypes => cellTypes; // 只读访问
        private TetrisFactory tetrisFactory = new TetrisFactory(); // 用于生成随机 Tetri 的工厂
        private List<Type> attributeTypes; // 用于随机替换单元格的类型
        private Stack<Tetri.Tetri> usedTetriHistory = new Stack<Tetri.Tetri>();
        TetrisResources()
        {
            InitializeAttributeTypes();
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
                var tetri = tetrisFactory.CreateRandomShape(); // 使用工厂的随机生成方法
                ReplaceSpecificCell(tetri, attributeType); // 确保包含该 attributeType
                unusedTetriList.Add(tetri);
            }

            // 生成剩余的随机 Tetri
            for (int i = attributeTypes.Count; i < initialTetriCount; i++)
            {
                var tetri = tetrisFactory.CreateRandomShape(); // 使用工厂的随机生成方法
                ReplaceRandomCell(tetri); // 随机替换单元格
                unusedTetriList.Add(tetri);
            }

            RecalculateCellTypes();
        }

        private void ReplaceSpecificCell(Tetri.Tetri tetri, Type attributeType)
        {
            ReplaceCell(tetri, attributeType);
        }

        private void ReplaceRandomCell(Tetri.Tetri tetri)
        {
            ReplaceCell(tetri);
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

        public void DrawRandomTetriFromUnusedList(int count)
        {
            if (unusedTetriList.Count < count)
            {
                // 如果unusedTetriList为空，将usedTetriList中的所有Tetri移动到unusedTetriList中
                unusedTetriList.AddRange(usedTetriList);
                usedTetriList.Clear();
            }

            // 随机抽取若干个Tetri
            List<Tetri.Tetri> drawnTetri = unusedTetriList.OrderBy(x => Guid.NewGuid()).Take(count).ToList();

            // 从unusedTetriList中移除抽取的Tetri，并添加到tetriList中
            foreach (var tetri in drawnTetri)
            {
                unusedTetriList.Remove(tetri);
                tetriList.Add(tetri);
            }

            RecalculateCellTypes();
        }

        public void AddUsableTetri(Tetri.Tetri tetri)
        {
            tetriList.Add(tetri);
            RecalculateCellTypes();
        }

        
        public void UseTetri(Tetri.Tetri tetri)
        {
            tetriList.Remove(tetri);
            if (!tetri.IsDisposable)
            {
                usedTetriList.Add(tetri);
                usedTetriHistory.Push(tetri); // 记录到历史栈

            }
            RecalculateCellTypes();
        }
        public void UndoLastUseTetri()
        {
            if (usedTetriHistory.Count > 0)
            {
                Tetri.Tetri lastUsedTetri = usedTetriHistory.Pop(); // 取出最近使用的 Tetri
                usedTetriList.Remove(lastUsedTetri); // 从已使用列表中移除
                tetriList.Add(lastUsedTetri); // 恢复到可用列表
                RecalculateCellTypes();
            }
            else
            {
                Debug.LogWarning("No Tetri to undo.");
            }
        }

        public List<Tetri.Tetri> GetUsableTetris()
        {
            return new List<Tetri.Tetri>(tetriList);
        }

        public List<Tetri.Tetri> GetUsedTetris()
        {
            return new List<Tetri.Tetri>(usedTetriList);
        }

        public List<Tetri.Tetri> GetUnusedTetris()
        {
            return new List<Tetri.Tetri>(unusedTetriList);
        }

        public bool IsEmpty()
        {
            return tetriList == null || tetriList.Count == 0;
        }

        public void Reset()
        {
            tetriList.Clear();
            usedTetriList.Clear();
            unusedTetriList.Clear();
            GenerateInitialTetris();
        }

        public void InitialUnusedTetris(List<Tetri.Tetri> tetris)
        {
            unusedTetriList.AddRange(tetris);
            RecalculateCellTypes();
        }

        private void RecalculateCellTypes()
        {
            cellTypes.Clear(); // 清空当前的 cellTypes

            // 遍历所有 Tetri 列表，重新计算包含的 Cell 类型
            foreach (var tetri in tetriList.Concat(unusedTetriList).Concat(usedTetriList))
            {
                foreach (var position in tetri.GetOccupiedPositions())
                {
                    Cell cell = tetri.Shape[position.x, position.y];
                    cellTypes.Add(cell.GetType());
                }
            }

            // 触发数据变化事件
            OnDataChanged?.Invoke();
        }

        internal void RemoveTetri(Tetri.Tetri tetri)
        {
            if (tetriList.Contains(tetri))
            {
                tetriList.Remove(tetri);
            }
            else if (usedTetriList.Contains(tetri))
            {
                usedTetriList.Remove(tetri);
            }
            else if (unusedTetriList.Contains(tetri))
            {
                unusedTetriList.Remove(tetri);
            }
        }

        internal void ClearHistory()
        {
            usedTetriHistory.Clear();
        }
    }
}
