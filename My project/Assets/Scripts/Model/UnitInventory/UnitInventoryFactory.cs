using System.Collections.Generic;
using Model;
using Model.Tetri;
using UnityEngine;

namespace Model.UnitInventory
{
    [CreateAssetMenu(fileName = "UnitInventoryFactory", menuName = "Game Data/Bootstrap/Factories/Unit Inventory Factory")]
    public class UnitInventoryFactory : ScriptableObject
    {
        [SerializeField] private TetriCellFactory tetriCellFactory;
        [SerializeField] private CharacterFactory characterFactory;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tetriCellFactory == null)
            {
                Debug.LogError($"[{nameof(UnitInventoryFactory)}] Tetri cell factory is missing.", this);
            }

            if (characterFactory == null)
            {
                Debug.LogError($"[{nameof(UnitInventoryFactory)}] Character factory is missing.", this);
            }
        }
#endif

        public List<UnitSpawnPlacement> Build(UnitInventoryBuildData buildData)
        {
            if (buildData == null)
            {
                return new List<UnitSpawnPlacement>();
            }

            var items = new List<UnitSpawnPlacement>();
            foreach (UnitInventoryBuildEntry entry in buildData.Entries)
            {
                if (entry == null || entry.CharacterDefinition == null)
                {
                    continue;
                }

                items.Add(BuildUnitSpawnPlacement(entry));
            }

            return items;
        }

        private UnitSpawnPlacement BuildUnitSpawnPlacement(UnitInventoryBuildEntry entry)
        {
            Character character = characterFactory.CreateCharacter(entry.CharacterDefinition);
            var cells = new List<Cell>();
            foreach (UnitInventoryCellBuildEntry cellEntry in entry.Cells)
            {
                Cell cell = tetriCellFactory.CreateCell(cellEntry.Definition);
                cell.Level = cellEntry.Level;
                cells.Add(cell);
            }

            var build = new CharacterBuild(character, cells, null);
            return new UnitSpawnPlacement(build, entry.RelativePosition);
        }
    }
}