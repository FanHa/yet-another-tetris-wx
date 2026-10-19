using System;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Bootstrap/Factories/Tetri Cell Factory")]
    public class TetriCellFactory : ScriptableObject
    {
        [SerializeField] private CellDatabase cellDatabase;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (cellDatabase == null)
            {
                Debug.LogError($"[{nameof(TetriCellFactory)}] Cell database is missing.", this);
            }
        }
#endif

        public Cell CreateFillerCell()
        {
            CellDefinition definition = cellDatabase.GetFillerDefinition();
            return CreateCell(definition);
        }

        public Cell CreateCell(string cellId)
        {
            if (string.IsNullOrWhiteSpace(cellId))
            {
                throw new ArgumentException("Cell id is null or empty.", nameof(cellId));
            }

            var definition = cellDatabase.GetDefinition(cellId);
            return CreateCell(definition);
        }

        public Cell CreateCell(CellDefinition definition)
        {
            var cell = definition switch
            {
                SkillBackedCellDefinition => new SkillCell(),
                _ => new Cell()
            };
            cell.Initialize(definition);
            return cell;
        }

        public Cell Clone(Cell cell)
        {
            var clone = CreateCell(cell.Definition);
            clone.Level = cell.Level;
            clone.Affinity = cell.Affinity;
            clone.Resonance = cell.Resonance;
            return clone;
        }
    }
}

