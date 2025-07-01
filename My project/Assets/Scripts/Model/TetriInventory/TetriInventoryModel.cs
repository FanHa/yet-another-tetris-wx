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
        [SerializeField] private Model.Tetri.TetriFactory tetriModelFactory;
        public IReadOnlyList<Model.Tetri.Tetri> UsableTetriList => usableTetriList;


        private readonly HashSet<CellTypeId> existCellTypeIds = new();
        public IReadOnlyCollection<CellTypeId> ExistCellTypeIds => existCellTypeIds;

        private readonly HashSet<CharacterTypeId> existCharacterTypeIds = new();
        public IReadOnlyCollection<CharacterTypeId> ExistCharacterTypeIds => existCharacterTypeIds;


        [SerializeField] private Model.Tetri.TetriCellFactory tetriCellFactory;
        [SerializeField] private List<TetriInventoryInitConfig> initialConfigs = new List<TetriInventoryInitConfig>();
        [SerializeField] private int avaliableConfigIndex; // 当前使用的配置索引


        public void Init()
        {
            // 先清空列表，避免重复
            usableTetriList.Clear();
            usedTetriList.Clear();
            GenerateInitialTetris();
        }


        private void GenerateInitialTetris()
        {
            TetriInventoryInitConfig config = initialConfigs[avaliableConfigIndex];

            List<CellTypeId> initialCellIds = config.CellTypeIds;
            List<CharacterTypeId> initialCharacterIds = config.CharacterTypeIds;

            // 遍历配置的每个特殊单元格类型
            foreach (var cellTypeId in initialCellIds)
            {
                Tetri.Tetri tetri = tetriModelFactory.CreateRandomShapeWithCell(cellTypeId);
                usableTetriList.Add(tetri);
            }

            foreach (var characterTypeId in initialCharacterIds) // 遍历新的列表
            {
                Tetri.Tetri characterTetri = tetriModelFactory.CreateCharacterTetri(characterTypeId);
                usableTetriList.Add(characterTetri);
            }

            RecalculateCellTypes(); // 所有 Tetri 添加完毕后，统一重新计算一次
        }

        private void RecalculateCellTypes()
        {
            existCellTypeIds.Clear(); 
            existCharacterTypeIds.Clear();

            // 遍历所有 Tetri 列表，重新计算包含的 CellTypeId
            foreach (var tetri in usableTetriList.Concat(usedTetriList))
            {
                foreach (var position in tetri.GetOccupiedPositions())
                {
                    Model.Tetri.Cell cell = tetri.Shape[position.x, position.y];
                    // 通过工厂的 Type->CellTypeId 映射获取 CellTypeId
                    if (tetriCellFactory.TypeToCellTypeId.TryGetValue(cell.GetType(), out var cellTypeId))
                    {
                        existCellTypeIds.Add(cellTypeId);
                    }
                }
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
                if (tetriCellFactory.TypeToCellTypeId.TryGetValue(cell.GetType(), out var cellTypeId))
                {
                    existCellTypeIds.Add(cellTypeId);
                }
            }
            // 触发数据变化事件
            OnDataChanged?.Invoke();
        }
        
        public List<Model.Tetri.Tetri> GetAllTetris()
        {
            // 返回所有 Tetri，包括可用和已使用的
            return usableTetriList.Concat(usedTetriList).ToList();
        }
    }
}