using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using Operation;
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

        private List<Type> attributeTypes= new(); // 用于随机替换单元格的类型
        private Model.Tetri.TetrisFactory tetrisFactory = new Model.Tetri.TetrisFactory();
        private HashSet<CellGroupConfig.Group> tetriGroups = new HashSet<CellGroupConfig.Group>();
        public IReadOnlyCollection<CellGroupConfig.Group> TetriGroups => tetriGroups; // 只读访问
        private HashSet<Type> cellTypes = new HashSet<Type>();

        [Header("初始属性Cell列表")] // 修改了 Inspector 标题
        [SerializeField] private List<CellTypeReference> initialSpecialCellTypes = new List<CellTypeReference>();

        [Header("初始角色Cell列表")] // 新的列表配置
        [SerializeField]
        private List<CharacterTypeReference> initialSingleCharacterCells = new List<CharacterTypeReference>(); // 修改为列表

        public void Init()
        {
            attributeTypes.Clear();
            InitializeAttributeTypes();
            // 先清空列表，避免重复
            usableTetriList.Clear();
            usedTetriList.Clear();
            GenerateInitialTetris();
        }

        private void InitializeAttributeTypes()
        {
            // 初始化 attributeTypes，仅在为空时加载一次
            attributeTypes = typeof(Model.Tetri.Attribute).Assembly
                .GetTypes()
                .Where(type => typeof(Model.Tetri.Attribute).IsAssignableFrom(type) // 检查是否实现了接口
                                && !type.IsAbstract // 排除抽象类
                                && !type.IsInterface) // 排除接口本身
                .ToList();
        }

        private void GenerateInitialTetris()
        {
            // 遍历配置的每个特殊单元格类型
            foreach (var cellTypeRef in initialSpecialCellTypes)
            {
                var tetri = tetrisFactory.CreateRandomBaseShape();
                if (tetri == null)
                {
                    Debug.LogWarning("TetrisFactory 返回了一个空的 Tetri。跳过此项。");
                    continue;
                }

                // 如果 CellTypeReference 有效并且其 Type 属性不为 null
                if (cellTypeRef != null && cellTypeRef.Type != null)
                {
                    try
                    {
                        // 尝试创建特殊单元格的实例
                        // 假设 CellTypeReference 有一个 CreateInstance() 方法，或者我们直接用 Type 创建
                        Model.Tetri.Cell specialCellInstance = null;
                        if (typeof(Model.Tetri.Cell).IsAssignableFrom(cellTypeRef.Type))
                        {
                             specialCellInstance = Activator.CreateInstance(cellTypeRef.Type) as Model.Tetri.Cell;
                        }
                        
                        if (specialCellInstance != null)
                        {
                            tetri.ReplaceRandomOccupiedCell(specialCellInstance);
                        }
                        else
                        {
                            Debug.LogWarning($"无法为类型 '{cellTypeRef.typeName}' 创建 Cell 实例，或该类型不是 Cell。Tetri 将不包含此特殊单元格。");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"创建 Cell 实例时出错 (类型: {cellTypeRef.typeName}): {ex.Message}");
                    }
                }
                // 如果 cellTypeRef 为 null 或者其 Type 为 null，
                // 则表示这个 Tetri 是普通的，不包含通过此配置指定的特殊单元格。
                
                usableTetriList.Add(tetri);
            }

            foreach (var charTypeRef in initialSingleCharacterCells) // 遍历新的列表
            {
                if (charTypeRef != null && charTypeRef.Type != null)
                {
                    var baseTetriForCharacter = tetrisFactory.CreateSinglePaddingCellTetri();
                    if (baseTetriForCharacter == null)
                    {
                         Debug.LogWarning("TetrisFactory.CreateSinglePaddingCellTetri() 返回了一个空的 Tetri。无法创建 Character Tetri。");
                         continue; // 跳过这个配置项
                    }
                    
                    try
                    {
                        // 假设 CharacterTypeReference 有 CreateInstance() 方法
                        Model.Tetri.Character characterInstance = charTypeRef.CreateInstance(); 
                        if (characterInstance != null)
                        {
                            baseTetriForCharacter.SetCell(1, 1, characterInstance);
                            usableTetriList.Add(baseTetriForCharacter);
                        }
                        else
                        {
                            Debug.LogWarning($"无法为类型 '{charTypeRef.typeName}' 创建 Character 实例。将不添加此 Character Tetri。");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"创建 Character 实例时出错 (类型: {charTypeRef.typeName}): {ex.Message}");
                    }
                }
                else
                {
                    // 如果 charTypeRef 为 null 或其 Type 为 null，可以选择创建一个默认的单格 Tetri (例如只有 Padding)
                    // 或者直接跳过，不添加任何东西。当前逻辑是跳过。
                    // Debug.Log("initialSingleCharacterCells 中的一个配置项无效，已跳过。");
                }
            }
            
            RecalculateCellTypes(); // 所有 Tetri 添加完毕后，统一重新计算一次
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
                    Model.Tetri.Cell cell = tetri.Shape[position.x, position.y];
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

        public void AddTetri(Tetri.Tetri modelTetri)
        {
            if (modelTetri == null)
            {
                Debug.LogWarning("Cannot add a null Tetri.");
                return;
            }

            // 添加到 usableTetriList
            usableTetriList.Add(modelTetri);

            // 更新 cellTypes 和 tetriGroups
            foreach (var position in modelTetri.GetOccupiedPositions())
            {
                Model.Tetri.Cell cell = modelTetri.Shape[position.x, position.y];
                cellTypes.Add(cell.GetType());
            }
            tetriGroups.Add(modelTetri.Group);

            // 触发数据变化事件
            OnDataChanged?.Invoke();
        }
    }
}