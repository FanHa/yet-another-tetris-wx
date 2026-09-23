using System.Collections.Generic;
using Model;
using Model.Tetri;
using UnityEngine;

namespace Model.UnitInventory
{
    [CreateAssetMenu(fileName = "UnitInventoryFactory", menuName = "Tetris/UnitInventoryFactory")]
    public class UnitInventoryFactory : ScriptableObject
    {
        [SerializeField] private TetriCellFactory tetriCellFactory;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tetriCellFactory == null)
            {
                Debug.LogError($"[{nameof(UnitInventoryFactory)}] Tetri cell factory is missing.", this);
            }
        }
#endif

        public List<CharacterPlacement> Build(UnitInventoryBuildData buildData)
        {
            if (buildData == null)
            {
                return new List<CharacterPlacement>();
            }

            var items = new List<CharacterPlacement>();
            foreach (UnitInventoryBuildEntry entry in buildData.Entries)
            {
                if (entry == null || entry.CharacterDefinition == null)
                {
                    continue;
                }

                items.Add(BuildCharacterPlacement(entry));
            }

            return items;
        }

        private CharacterPlacement BuildCharacterPlacement(UnitInventoryBuildEntry entry)
        {
            Character character = tetriCellFactory.CreateCharacterCell(entry.CharacterDefinition);
            var cells = new List<Cell>();
            foreach (UnitInventoryCellBuildEntry cellEntry in entry.Cells)
            {
                Cell cell = tetriCellFactory.CreateCell(cellEntry.Definition);
                cell.Level = cellEntry.Level;
                cells.Add(cell);
            }

            var influence = new CharacterInfluence(character, cells, null);
            return new CharacterPlacement(influence, entry.RelativePosition);
        }
    }
}