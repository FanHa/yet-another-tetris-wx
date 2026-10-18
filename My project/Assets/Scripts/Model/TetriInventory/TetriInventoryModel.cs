using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "TetriInventoryModel", menuName = "Game Data/Gameplay/Inventory/NormalMode/Runtime Inventory", order = 1)]

    public class TetriInventoryModel : ScriptableObject
    {
        public event Action OnDataChanged;

        private readonly List<Model.Tetri.Tetri> usableTetriList = new();
        private readonly List<Model.Tetri.Tetri> usedTetriList = new();
        [SerializeField] private Model.Tetri.TetriFactory tetriModelFactory;
        public IReadOnlyList<Model.Tetri.Tetri> UsableTetriList => usableTetriList;


        private readonly HashSet<CellDefinition> ownedCellDefinitions = new();
        public IReadOnlyCollection<CellDefinition> OwnedCellDefinitions => ownedCellDefinitions;

        private readonly HashSet<CharacterDefinition> ownedCharacterDefinitions = new();
        public IReadOnlyCollection<CharacterDefinition> OwnedCharacterDefinitions => ownedCharacterDefinitions;

        [SerializeField] private TetriInventoryInitConfig initialConfig;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tetriModelFactory == null)
            {
                Debug.LogError($"[{nameof(TetriInventoryModel)}] Tetri model factory is missing.", this);
            }

            if (initialConfig == null)
            {
                Debug.LogError($"[{nameof(TetriInventoryModel)}] Initial inventory config is missing.", this);
            }
        }
#endif


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
            List<CharacterDefinition> initialCharacterDefinitions = initialConfig.CharacterDefinitions;

            foreach (var cellDefinition in initialCellDefinitions)
            {
                Tetri.Tetri tetri = tetriModelFactory.CreateRandomCellBasedTetri(cellDefinition);
                AddTetri(tetri, silent: true); 
            }

            foreach (var characterDefinition in initialCharacterDefinitions)
            {
                Tetri.Tetri characterTetri = tetriModelFactory.CreateCharacterTetri(characterDefinition);
                AddTetri(characterTetri, silent: true);
            }

            RecalculateOwnedDefinitions();
            OnDataChanged?.Invoke(); 
        }

        private void RecalculateOwnedDefinitions()
        {
            ownedCellDefinitions.Clear();
            ownedCharacterDefinitions.Clear();
            foreach (var tetri in usableTetriList.Concat(usedTetriList))
                UpdateOwnedDefinitionsForTetri(tetri);
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
            UpdateOwnedDefinitionsForTetri(modelTetri);
            // 触发数据变化事件
            if (!silent)
                OnDataChanged?.Invoke();
        }

        private void UpdateOwnedDefinitionsForTetri(Tetri.Tetri tetri)
        {
            switch (tetri)
            {
                case CellBasedTetri cellBasedTetri:
                    foreach (var cell in cellBasedTetri.Cells.Values)
                    {
                        ownedCellDefinitions.Add(cell.Definition);
                    }
                    break;
                case CharacterTetri characterTetri:
                    ownedCharacterDefinitions.Add(characterTetri.Character.DefinitionData);
                    break;
                default:
                    throw new ArgumentException("Unsupported Tetri type.", nameof(tetri));
            }
        }

        public void RemoveTetri(Tetri.Tetri tetri)
        {
            if (usableTetriList.Remove(tetri) || usedTetriList.Remove(tetri))
            {
                // 解绑事件
                tetri.OnDataChanged -= HandleTetriChanged;
                RecalculateOwnedDefinitions();
                OnDataChanged?.Invoke();
            }
        }

        private void HandleTetriChanged()
        {
            RecalculateOwnedDefinitions();
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