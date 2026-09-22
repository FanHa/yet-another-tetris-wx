using System.Collections.Generic;
using System.Linq;
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
            if (config == null)
            {
                return new List<CharacterPlacement>();
            }

            return Build(config.Items
                .Where(itemConfig => itemConfig != null && itemConfig.characterDefinition != null)
                .Select(itemConfig => new UnitInventoryBuildEntry(
                    itemConfig.characterDefinition,
                    itemConfig.cellDefinitions == null
                        ? Enumerable.Empty<(CellDefinition Definition, int Level)>()
                        : itemConfig.cellDefinitions
                            .Where(cellConfig => cellConfig != null && cellConfig.definition != null)
                            .Select(cellConfig => (cellConfig.definition, cellConfig.level)),
                    itemConfig.relativePositionFromCenter)));
        }

        public List<CharacterPlacement> Build(
            IReadOnlyList<EnemySquadData> enemySquads)
        {
            if (enemySquads == null)
            {
                return new List<CharacterPlacement>();
            }

            return Build(enemySquads
                .Where(enemySquad => enemySquad != null && enemySquad.characterDefinition != null)
                .Select(enemySquad => new UnitInventoryBuildEntry(
                    enemySquad.characterDefinition,
                    enemySquad.cellConfigs == null
                        ? Enumerable.Empty<(CellDefinition Definition, int Level)>()
                        : enemySquad.cellConfigs
                            .Where(cellConfig => cellConfig != null && cellConfig.definition != null)
                            .Select(cellConfig => (cellConfig.definition, cellConfig.level)),
                    Vector3.zero)));
        }

        public List<CharacterPlacement> Build(
            IEnumerable<UnitInventoryBuildEntry> entries)
        {
            if (entries == null)
            {
                return new List<CharacterPlacement>();
            }

            var items = new List<CharacterPlacement>();
            foreach (UnitInventoryBuildEntry entry in entries)
            {
                if (entry == null || entry.CharacterDefinition == null)
                {
                    continue;
                }

                items.Add(BuildCharacterPlacement(entry.CharacterDefinition, entry.CellEntries, entry.RelativePosition));
            }

            return items;
        }

        private CharacterPlacement BuildCharacterPlacement(
            CharacterDefinition characterDefinition,
            IEnumerable<(CellDefinition Definition, int Level)> cellEntries,
            Vector3 relativePosition)
        {
            Character character = tetriCellFactory.CreateCharacterCell(characterDefinition);
            var cells = new List<Cell>();
            foreach (var cellEntry in cellEntries)
            {
                Cell cell = tetriCellFactory.CreateCell(cellEntry.Definition);
                cell.Level = cellEntry.Level;
                cells.Add(cell);
            }

            var influence = new CharacterInfluence(character, cells, null);
            return new CharacterPlacement(influence, relativePosition);
        }

        public sealed class UnitInventoryBuildEntry
        {
            public UnitInventoryBuildEntry(
                CharacterDefinition characterDefinition,
                IEnumerable<(CellDefinition Definition, int Level)> cellEntries,
                Vector3 relativePosition)
            {
                CharacterDefinition = characterDefinition;
                CellEntries = cellEntries ?? Enumerable.Empty<(CellDefinition Definition, int Level)>();
                RelativePosition = relativePosition;
            }

            public CharacterDefinition CharacterDefinition { get; }
            public IEnumerable<(CellDefinition Definition, int Level)> CellEntries { get; }
            public Vector3 RelativePosition { get; }
        }
    }
}