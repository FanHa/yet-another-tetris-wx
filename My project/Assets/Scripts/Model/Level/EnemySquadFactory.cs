using System;
using System.Collections.Generic;
using Model.Tetri;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "EnemySquadFactory", menuName = "Game Data/Bootstrap/Factories/Enemy Squad Factory")]
    public class EnemySquadFactory : ScriptableObject
    {
        [SerializeField] private CellDatabase cellDatabase;
        [SerializeField] private CharacterDatabase characterDatabase;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (cellDatabase == null)
            {
                Debug.LogError($"[{nameof(EnemySquadFactory)}] Cell database is missing.", this);
            }

            if (characterDatabase == null)
            {
                Debug.LogError($"[{nameof(EnemySquadFactory)}] Character database is missing.", this);
            }
        }
#endif

        public List<EnemySquadData> Build(LevelState levelState)
        {
            if (levelState == null)
            {
                throw new ArgumentNullException(nameof(levelState));
            }

            var availableCharacterDefinitions = characterDatabase.GetRegisteredDefinitions();

            var availableCellDefinitions = cellDatabase.GetRegisteredDefinitions();

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
