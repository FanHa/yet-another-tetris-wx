using System;
using System.Collections.Generic;
using System.Linq;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    public class EnemySquadFactory
    {
        private readonly CellDatabase cellDatabase;

        public EnemySquadFactory(CellDatabase cellDatabase)
        {
            this.cellDatabase = cellDatabase;
        }

        public List<EnemySquadData> Build(LevelState levelState)
        {
            if (levelState == null)
            {
                throw new ArgumentNullException(nameof(levelState));
            }

            if (cellDatabase == null)
            {
                throw new InvalidOperationException("CellDatabase is missing for enemy squad generation.");
            }

            var availableCharacterDefinitions = cellDatabase
                .GetRegisteredDefinitions()
                .OfType<CharacterDefinition>()
                .ToList();

            var availableCellDefinitions = cellDatabase
                .GetRegisteredDefinitions()
                .Where(definition => definition != null && definition is not CharacterDefinition)
                .ToList();

            var squads = new List<EnemySquadData>();
            int enemyCount = Mathf.Min(1 + (levelState.CurrentLevel - 1) / levelState.LevelsPerEnemyIncrease, levelState.MaxEnemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                CharacterDefinition selectedCharacter = availableCharacterDefinitions.Count > 0
                    ? availableCharacterDefinitions[UnityEngine.Random.Range(0, availableCharacterDefinitions.Count)]
                    : null;

                if (selectedCharacter == null)
                {
                    continue;
                }

                var squad = new EnemySquadData
                {
                    characterDefinition = selectedCharacter,
                    cellConfigs = new List<EnemyCellData>()
                };

                int additionalCells = Mathf.Min((levelState.CurrentLevel - 1) / levelState.LevelsPerCellIncrease + 1, levelState.MaxAddedCellCount);
                for (int cellIndex = 0; cellIndex < additionalCells; cellIndex++)
                {
                    if (availableCellDefinitions.Count == 0)
                    {
                        break;
                    }

                    CellDefinition selectedDefinition = availableCellDefinitions[UnityEngine.Random.Range(0, availableCellDefinitions.Count)];
                    squad.cellConfigs.Add(new EnemyCellData
                    {
                        definition = selectedDefinition,
                        level = 1
                    });
                }

                squads.Add(squad);
            }

            return squads;
        }
    }
}
