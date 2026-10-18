using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Game Data/Gameplay/Characters/Character Database")]
    public sealed class CharacterDatabase : ScriptableObject
    {
        [SerializeField] private List<CharacterDefinition> registeredCharacterDefinitions = new();

        private Dictionary<string, CharacterDefinition> definitionById;

        public IReadOnlyList<CharacterDefinition> RegisteredDefinitions => registeredCharacterDefinitions.AsReadOnly();

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
            definitionById = new Dictionary<string, CharacterDefinition>(StringComparer.Ordinal);
            foreach (var definition in registeredCharacterDefinitions)
            {
                definitionById.Add(definition.Id, definition);
            }
        }

        public CharacterDefinition GetDefinition(string id)
        {
            return definitionById[id];
        }

        public List<CharacterDefinition> GetRegisteredDefinitions()
        {
            return new List<CharacterDefinition>(registeredCharacterDefinitions);
        }

        public List<string> GetRegisteredCharacterIds()
        {
            return registeredCharacterDefinitions.Select(definition => definition.Id).ToList();
        }

        public bool ValidateConfiguration()
        {
            if (registeredCharacterDefinitions == null)
            {
                Debug.LogError($"{nameof(registeredCharacterDefinitions)} is null.", this);
                return false;
            }

            bool isValid = true;
            var seenIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var definition in registeredCharacterDefinitions)
            {
                if (definition == null)
                {
                    Debug.LogError($"{nameof(registeredCharacterDefinitions)} contains a null definition.", this);
                    isValid = false;
                    continue;
                }

                if (!definition.ValidateConfiguration())
                {
                    isValid = false;
                }

                if (!seenIds.Add(definition.Id))
                {
                    Debug.LogError($"Duplicate CharacterDefinition ID '{definition.Id}'.", this);
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}