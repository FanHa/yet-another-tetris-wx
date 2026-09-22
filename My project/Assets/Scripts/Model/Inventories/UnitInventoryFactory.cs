using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
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

        public List<CharacterPlacement> Build(
            UnitInventoryInitConfig config)
        {
            var items = new List<CharacterPlacement>(config.Items.Count);
            foreach (UnitInventoryItemInitConfig itemConfig in config.Items)
            {
                Character character = tetriCellFactory.CreateCharacterCell(itemConfig.characterDefinition);
                var cells = new List<Cell>(itemConfig.cellDefinitions.Count);
                foreach (UnitInventoryCellInitConfig cellConfig in itemConfig.cellDefinitions)
                {
                    Cell cell = tetriCellFactory.CreateCell(cellConfig.definition);
                    cell.Level = cellConfig.level;
                    cells.Add(cell);
                }

                var influence = new CharacterInfluence(character, cells, null);
                items.Add(new CharacterPlacement(influence, itemConfig.relativePositionFromCenter));
            }

            return items;
        }
    }
}