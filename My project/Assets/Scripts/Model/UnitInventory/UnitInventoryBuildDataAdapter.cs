using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace Model.UnitInventory
{
    public static class UnitInventoryBuildDataAdapter
    {
        public static UnitInventoryBuildData FromInitConfig(UnitInventoryInitConfig config)
        {
            if (config == null)
            {
                return Empty();
            }

            return new UnitInventoryBuildData(config.Items
                .Where(itemConfig => itemConfig != null && itemConfig.characterDefinition != null)
                .Select(itemConfig => new UnitInventoryBuildEntry(
                    itemConfig.characterDefinition,
                    itemConfig.cellDefinitions == null
                        ? Enumerable.Empty<UnitInventoryCellBuildEntry>()
                        : itemConfig.cellDefinitions
                            .Where(cellConfig => cellConfig != null && cellConfig.definition != null)
                            .Select(cellConfig => new UnitInventoryCellBuildEntry(
                                cellConfig.definition,
                                cellConfig.level)),
                    itemConfig.relativePositionFromCenter)));
        }

        public static UnitInventoryBuildData FromEnemySquads(
            IReadOnlyList<EnemySquadData> enemySquads)
        {
            if (enemySquads == null)
            {
                return Empty();
            }

            return new UnitInventoryBuildData(enemySquads
                .Where(enemySquad => enemySquad != null && enemySquad.characterDefinition != null)
                .Select(enemySquad => new UnitInventoryBuildEntry(
                    enemySquad.characterDefinition,
                    enemySquad.cellConfigs == null
                        ? Enumerable.Empty<UnitInventoryCellBuildEntry>()
                        : enemySquad.cellConfigs
                            .Where(cellConfig => cellConfig != null && cellConfig.definition != null)
                            .Select(cellConfig => new UnitInventoryCellBuildEntry(
                                cellConfig.definition,
                                cellConfig.level)),
                    Vector3.zero)));
        }

        private static UnitInventoryBuildData Empty()
        {
            return new UnitInventoryBuildData(Array.Empty<UnitInventoryBuildEntry>());
        }
    }
}