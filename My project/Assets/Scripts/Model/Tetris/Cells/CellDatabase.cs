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
        private Dictionary<string, Type> cellTypeById;
        private Dictionary<Type, string> idByCellType;

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
            cellTypeById = new Dictionary<string, Type>(StringComparer.Ordinal);
            idByCellType = new Dictionary<Type, string>();

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

                if (!definition.TryResolveCellType(out Type cellType, out _))
                {
                    continue;
                }

                cellTypeById[definition.Id] = cellType;
                idByCellType[cellType] = definition.Id;
            }
        }

        public bool TryGetDefinition(string id, out CellDefinition definition)
        {
            EnsureInitialized();
            return definitionById.TryGetValue(id, out definition);
        }

        public bool TryGetCellType(string id, out Type cellType)
        {
            EnsureInitialized();
            return cellTypeById.TryGetValue(id, out cellType);
        }

        public bool TryGetId(Type cellType, out string id)
        {
            EnsureInitialized();

            if (cellType == null)
            {
                id = null;
                return false;
            }

            return idByCellType.TryGetValue(cellType, out id);
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

        public bool TryGetSprite(Type cellType, out Sprite sprite)
        {
            EnsureInitialized();
            sprite = null;

            if (cellType == null)
            {
                return false;
            }

            if (!TryGetId(cellType, out string id) || string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            return TryGetSprite(id, out sprite);
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
            return definitionById.Keys
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToList();
        }

        private void EnsureInitialized()
        {
            if (definitionById != null && cellTypeById != null && idByCellType != null)
            {
                return;
            }

            InitializeIndexes();
        }
    }
}