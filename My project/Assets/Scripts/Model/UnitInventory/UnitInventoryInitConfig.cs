using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Model.UnitInventory
{
    [CreateAssetMenu(fileName = "UnitInventoryInitConfig", menuName = "Game Data/Gameplay/Inventory/Units/Init Config")]
    public class UnitInventoryInitConfig : ScriptableObject
    {
        [SerializeField] private List<UnitInventoryItemInitConfig> items = new();

        public IReadOnlyList<UnitInventoryItemInitConfig> Items => items;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (items == null)
            {
                Debug.LogError($"[{nameof(UnitInventoryInitConfig)}] Items list is missing.", this);
                return;
            }

            for (int itemIndex = 0; itemIndex < items.Count; itemIndex++)
            {
                UnitInventoryItemInitConfig item = items[itemIndex];
                if (item == null)
                {
                    Debug.LogError($"[{nameof(UnitInventoryInitConfig)}] Item at index {itemIndex} is missing.", this);
                    continue;
                }

                if (item.characterDefinition == null)
                {
                    Debug.LogError($"[{nameof(UnitInventoryInitConfig)}] Item at index {itemIndex} is missing its character definition.", this);
                }

                if (item.cellDefinitions == null)
                {
                    Debug.LogError($"[{nameof(UnitInventoryInitConfig)}] Item at index {itemIndex} has no cell definitions list.", this);
                    continue;
                }

                for (int cellIndex = 0; cellIndex < item.cellDefinitions.Count; cellIndex++)
                {
                    UnitInventoryCellInitConfig cell = item.cellDefinitions[cellIndex];
                    if (cell == null || cell.definition == null)
                    {
                        Debug.LogError($"[{nameof(UnitInventoryInitConfig)}] Item at index {itemIndex} has an invalid cell at index {cellIndex}.", this);
                        continue;
                    }

                    if (cell.definition is CharacterDefinition)
                    {
                        Debug.LogError($"[{nameof(UnitInventoryInitConfig)}] Item at index {itemIndex} uses character definition '{cell.definition.name}' as an influenced cell.", this);
                    }

                    if (cell.level < 1)
                    {
                        Debug.LogError($"[{nameof(UnitInventoryInitConfig)}] Cell '{cell.definition.name}' has invalid level {cell.level}.", this);
                    }
                }

                var duplicateDefinitions = item.cellDefinitions
                    .Where(cell => cell?.definition != null)
                    .GroupBy(cell => cell.definition)
                    .Where(group => group.Count() > 1);
                foreach (var duplicateDefinition in duplicateDefinitions)
                {
                    Debug.LogError($"[{nameof(UnitInventoryInitConfig)}] Item at index {itemIndex} contains duplicate cell definition '{duplicateDefinition.Key.name}'.", this);
                }
            }
        }
#endif
    }

    [System.Serializable]
    public class UnitInventoryItemInitConfig
    {
        public CharacterDefinition characterDefinition;
        public List<UnitInventoryCellInitConfig> cellDefinitions = new();
        public Vector3 relativePositionFromCenter;
    }

    [System.Serializable]
    public class UnitInventoryCellInitConfig
    {
        public CellDefinition definition;
        public int level = 1;
    }
}