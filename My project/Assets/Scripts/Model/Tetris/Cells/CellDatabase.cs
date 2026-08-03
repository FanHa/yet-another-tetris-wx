using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(fileName = "CellDatabase", menuName = "Config/Cells/Cell Database")]
    public sealed class CellDatabase : ScriptableObject
    {
        [SerializeField] private List<CellDefinition> definitions = new();

        private Dictionary<string, CellDefinition> definitionById;

        public IReadOnlyList<CellDefinition> Definitions => definitions;

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

            if (definitions == null)
            {
                return;
            }

            foreach (CellDefinition definition in definitions)
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

        public bool TryGetSprite(string id, out Sprite sprite)
        {
            EnsureInitialized();
            sprite = null;

            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            if (!definitionById.TryGetValue(id, out CellDefinition definition) || definition == null)
            {
                return false;
            }

            sprite = definition.Icon;
            return sprite != null;
        }

        public List<CellDefinition> GetDefinitions()
        {
            if (definitions == null)
            {
                return new List<CellDefinition>();
            }

            return new List<CellDefinition>(definitions);
        }

        public List<string> GetRegisteredCellIds()
        {
            EnsureInitialized();
            return definitions
                .Where(definition => definition != null && !string.IsNullOrWhiteSpace(definition.Id))
                .Select(definition => definition.Id)
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