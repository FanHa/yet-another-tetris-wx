using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "CellDatabase", menuName = "Game Data/Gameplay/Cells/Cell Database")]
    public sealed class CellDatabase : ScriptableObject
    {
        [SerializeField] private FillerCellDefinition fillerDefinition;
        [SerializeField] private List<CellDefinition> registeredCellDefinitions = new();

        private Dictionary<string, CellDefinition> definitionById;

        public IReadOnlyList<CellDefinition> RegisteredDefinitions => registeredCellDefinitions;

        private void OnEnable()
        {
            RebuildIndexes();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ValidateConfiguration())
            {
                RebuildIndexes();
            }
        }
#endif

        private void RebuildIndexes()
        {
            definitionById = new Dictionary<string, CellDefinition>(StringComparer.Ordinal);

            foreach (CellDefinition definition in registeredCellDefinitions)
            {
                definitionById.Add(definition.Id, definition);
            }
        }

#if UNITY_EDITOR
        private bool ValidateConfiguration()
        {
            bool isValid = true;

            if (fillerDefinition == null)
            {
                Debug.LogError($"{nameof(fillerDefinition)} is not assigned.", this);
                isValid = false;
            }

            if (registeredCellDefinitions == null)
            {
                Debug.LogError($"{nameof(registeredCellDefinitions)} is null.", this);
                return false;
            }

            HashSet<string> seenIds = new(StringComparer.Ordinal);
            foreach (CellDefinition definition in registeredCellDefinitions)
            {
                if (definition == null)
                {
                    Debug.LogError($"{nameof(registeredCellDefinitions)} contains a null definition.", this);
                    isValid = false;
                    continue;
                }

                string id = definition.Id;
                if (!string.IsNullOrWhiteSpace(id) && !seenIds.Add(id))
                {
                    Debug.LogError($"Duplicate CellDefinition ID '{id}'.", this);
                    isValid = false;
                }
            }

            return isValid;
        }
#endif

        public CellDefinition GetDefinition(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Cell id is null or empty.", nameof(id));
            }

            if (!definitionById.TryGetValue(id, out CellDefinition definition) || definition == null)
            {
                throw new KeyNotFoundException($"Unknown cell id: {id}");
            }

            return definition;
        }

        public CellDefinition GetFillerDefinition()
        {
            return fillerDefinition;
        }

        public Sprite GetSprite(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            if (!definitionById.TryGetValue(id, out CellDefinition definition) || definition == null)
            {
                return null;
            }

            Sprite sprite = ResolveSprite(definition);
            return sprite;
        }

        private Sprite ResolveSprite(CellDefinition definition)
        {
            if (definition == null)
            {
                return null;
            }

            return definition.Icon;

        }

        public List<CellDefinition> GetRegisteredDefinitions()
        {
            return new List<CellDefinition>(registeredCellDefinitions);
        }

        public List<string> GetRegisteredCellIds()
        {
            return registeredCellDefinitions
                .Where(definition => definition != null && !string.IsNullOrWhiteSpace(definition.Id))
                .Select(definition => definition.Id)
                .ToList();
        }

        public List<string> GetRegisteredCharacterIds()
        {
            return registeredCellDefinitions
                .Where(definition => definition is CharacterDefinition)
                .Select(definition => definition.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }
    }
}