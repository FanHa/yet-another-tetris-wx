using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Tetri;
using UnityEngine;

namespace Model.UnitInventory
{
    public sealed class UnitInventoryBuildData
    {
        public UnitInventoryBuildData(IEnumerable<UnitInventoryBuildEntry> entries)
        {
            Entries = entries?.ToArray() ?? System.Array.Empty<UnitInventoryBuildEntry>();
        }

        public IReadOnlyList<UnitInventoryBuildEntry> Entries { get; }
    }

    public sealed class UnitInventoryBuildEntry
    {
        public UnitInventoryBuildEntry(
            CharacterDefinition characterDefinition,
            IEnumerable<UnitInventoryCellBuildEntry> cells,
            Vector3 relativePosition)
        {
            CharacterDefinition = characterDefinition;
            Cells = cells?.ToArray() ?? System.Array.Empty<UnitInventoryCellBuildEntry>();
            RelativePosition = relativePosition;
        }

        public CharacterDefinition CharacterDefinition { get; }
        public IReadOnlyList<UnitInventoryCellBuildEntry> Cells { get; }
        public Vector3 RelativePosition { get; }
    }

    public sealed class UnitInventoryCellBuildEntry
    {
        public UnitInventoryCellBuildEntry(CellDefinition definition, int level)
        {
            Definition = definition;
            Level = level;
        }

        public CellDefinition Definition { get; }
        public int Level { get; }
    }
}