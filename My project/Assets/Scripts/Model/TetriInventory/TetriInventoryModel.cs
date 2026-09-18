using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

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


        private readonly HashSet<string> existCellIds = new(StringComparer.Ordinal);
        public IReadOnlyCollection<string> ExistCellIds => existCellIds;

        private readonly HashSet<CharacterDefinition> existCharacterDefinitions = new();
        public IReadOnlyCollection<CharacterDefinition> ExistCharacterDefinitions => existCharacterDefinitions;

        [SerializeField] private TetriInventoryInitConfig initialConfig;


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
            List<CellDefinition> initialCellDefinitions = initialConfig.CellDefinitions;
            List<CharacterDefinition> initialCharacterIds = initialConfig.CharacterDefinitions;

            foreach (var cellDefinition in initialCellDefinitions)
            {
                Tetri.Tetri tetri = tetriModelFactory.CreateRandomShapeWithCell(cellDefinition);
                AddTetri(tetri, silent: true); 
            }

            foreach (var characterDefinition in initialCharacterIds)
            {
                Tetri.Tetri characterTetri = tetriModelFactory.CreateCharacterTetri(characterDefinition);
                AddTetri(characterTetri, silent: true);
            }

            RecalculateCellIds(); 
            OnDataChanged?.Invoke(); 
        }

        private void RecalculateCellIds()
        {
            existCellIds.Clear();
            existCharacterDefinitions.Clear();
            foreach (var tetri in usableTetriList.Concat(usedTetriList))
                UpdateCellIdsForTetri(tetri);
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
            UpdateCellIdsForTetri(modelTetri);
            // 触发数据变化事件
            if (!silent)
                OnDataChanged?.Invoke();
        }

        private void UpdateCellIdsForTetri(Tetri.Tetri tetri)
        {
            foreach (var position in tetri.GetOccupiedPositions())
            {
                Model.Tetri.Cell cell = tetri.Shape[position.x, position.y];
                if (cell is Character character)
                {
                    existCharacterDefinitions.Add(character.DefinitionData);
                }
                else
                {
                    // 普通方块按 CellDefinition 体系的运行时 CellId 建立索引。
                    existCellIds.Add(cell.CellId);
                }
            }
        }

        public void RemoveTetri(Tetri.Tetri tetri)
        {
            if (usableTetriList.Remove(tetri) || usedTetriList.Remove(tetri))
            {
                // 解绑事件
                tetri.OnDataChanged -= HandleTetriChanged;
                RecalculateCellIds(); // 重新计算 CellIds
                OnDataChanged?.Invoke();
            }
        }

        private void HandleTetriChanged()
        {
            RecalculateCellIds(); // 重新计算 CellIds
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