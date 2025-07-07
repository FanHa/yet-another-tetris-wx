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

        [SerializeField] private List<TetriInventoryInitConfig> initialConfigs = new List<TetriInventoryInitConfig>();
        [SerializeField] private int avaliableConfigIndex; // 当前使用的配置索引


        public void Init()
        {
            foreach (var tetri in usableTetriList.Concat(usedTetriList))
                tetri.OnDataChanged -= HandleTetriChanged;
            usableTetriList.Clear();
            usedTetriList.Clear();
            GenerateInitialTetris();
        }


        private void GenerateInitialTetris()
        {
            TetriInventoryInitConfig config = initialConfigs[avaliableConfigIndex];

            List<CellTypeId> initialCellIds = config.CellTypeIds;
            List<CharacterTypeId> initialCharacterIds = config.CharacterTypeIds;

            foreach (var cellTypeId in initialCellIds)
            {
                Tetri.Tetri tetri = tetriModelFactory.CreateRandomShapeWithCell(cellTypeId);
                AddTetri(tetri, silent: true); 
            }

            foreach (var characterTypeId in initialCharacterIds)
            {
                Tetri.Tetri characterTetri = tetriModelFactory.CreateCharacterTetri(characterTypeId);
                AddTetri(characterTetri, silent: true);
            }

            RecalculateCellTypes(); 
            OnDataChanged?.Invoke(); 
        }

        private void RecalculateCellTypes()
        {
            existCellTypeIds.Clear();
            existCharacterTypeIds.Clear();
            foreach (var tetri in usableTetriList.Concat(usedTetriList))
                UpdateCellTypesForTetri(tetri);
        }

        public void MarkTetriAsUsed(Model.Tetri.Tetri tetri)
        {
            usableTetriList.Remove(tetri);
            usedTetriList.Add(tetri);
            // 触发数据变化事件
            OnDataChanged?.Invoke();
        }

        public void AddTetriRange(IEnumerable<Model.Tetri.Tetri> tetris)
        {
            foreach (var tetri in tetris)
            {
                AddTetri(tetri, silent: true);
            }

            // 触发数据变化事件
            OnDataChanged?.Invoke();
        }

        public void AddTetri(Tetri.Tetri modelTetri, bool silent = false)
        {
            // 添加到 usableTetriList
            usableTetriList.Add(modelTetri);
            modelTetri.OnDataChanged += HandleTetriChanged;
            UpdateCellTypesForTetri(modelTetri);
            // 触发数据变化事件
            if (!silent)
                OnDataChanged?.Invoke();
        }

        private void UpdateCellTypesForTetri(Tetri.Tetri tetri)
        {
            foreach (var position in tetri.GetOccupiedPositions())
            {
                Model.Tetri.Cell cell = tetri.Shape[position.x, position.y];
                if (cell is Character character)
                {
                    // 如果是角色类型，添加角色类型 ID
                    existCharacterTypeIds.Add(character.CharacterTypeId);
                }
                else
                {
                    // 如果是普通方块类型，添加方块类型 ID
                    existCellTypeIds.Add(cell.CellTypeId);
                }
            }
        }

        public void RemoveTetri(Tetri.Tetri tetri)
        {
            if (usableTetriList.Remove(tetri) || usedTetriList.Remove(tetri))
            {
                // 解绑事件
                tetri.OnDataChanged -= HandleTetriChanged;
                RecalculateCellTypes(); // 重新计算 CellTypeIds
                OnDataChanged?.Invoke();
            }
        }

        private void HandleTetriChanged()
        {
            RecalculateCellTypes(); // 重新计算 CellTypeIds
            // 处理 Tetri 数据变化
            OnDataChanged?.Invoke();
        }

        public List<Model.Tetri.Tetri> GetAllTetris()
        {
            // 返回所有 Tetri，包括可用和已使用的
            return usableTetriList.Concat(usedTetriList).ToList();
        }        
    }
}