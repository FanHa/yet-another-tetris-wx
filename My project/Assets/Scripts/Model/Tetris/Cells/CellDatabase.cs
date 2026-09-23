using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "CellDatabase", menuName = "Game Data/Gameplay/Cells/Cell Database")]
    public sealed class CellDatabase : ScriptableObject
    {

        [SerializeField] private CellDefinition paddingDefinition;
        [SerializeField] private List<CellDefinition> registeredCellDefinitions = new();

        private Dictionary<string, CellDefinition> definitionById;

        public IReadOnlyList<CellDefinition> RegisteredDefinitions => registeredCellDefinitions;

        private void OnEnable()
        {
            InitializeIndexes();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            InitializeIndexes();
        }
#endif

        private void InitializeIndexes()
        {
            definitionById = new Dictionary<string, CellDefinition>(StringComparer.Ordinal);

            foreach (CellDefinition definition in registeredCellDefinitions)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Id))
                {
                    continue;
                }

                definitionById[definition.Id] = definition;
            }
        }

        public CellDefinition GetDefinition(string id)
        {
            EnsureInitialized();

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

        public CellDefinition GetPaddingDefinition()
        {
            return paddingDefinition;
        }

        public Sprite GetSprite(string id)
        {
            EnsureInitialized();

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

        public Sprite GetSprite(Cell cell)
        {
            if (cell == null)
            {
                return null;
            }

            return GetSprite(cell.CellId);
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
            EnsureInitialized();
            return registeredCellDefinitions
                .Where(definition => definition != null && !string.IsNullOrWhiteSpace(definition.Id))
                .Select(definition => definition.Id)
                .ToList();
        }

        public List<string> GetRegisteredCharacterIds()
        {
            EnsureInitialized();
            return registeredCellDefinitions
                .Where(definition => definition is CharacterDefinition)
                .Select(definition => definition.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .ToList();
        }

        private void EnsureInitialized()
        {
            if (definitionById != null)
            {
                return;
            }

            InitializeIndexes();
        }
    }
}